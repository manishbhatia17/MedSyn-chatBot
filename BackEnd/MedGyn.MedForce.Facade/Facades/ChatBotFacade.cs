using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Factories.Interfaces;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace MedGyn.MedForce.Facade.Facades
{
    public class ChatBotFacade : IChatBotFacade
    {
        private readonly IChatBotService _chatBotService;
        private readonly ILLMService _llmService;
        private readonly ICustomerChatBotCommandHandlerFactory _handlerFactory;
        private readonly IMemoryCache _cache;
        private readonly ICustomerOrderFacade _customerOrderFacade;

        private const string FunctionDeclCacheKey = "ClaudeFunctionDeclarations";

        private static readonly HashSet<string> AllowedFunctionHints = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "GetProductByName",
            "GetCustomerPO",
            "GetCustomerOrderByEmail",
            "GetRepersentativeByCountryOrState",
            "LeaveMessageForMedGyn",
            "leave_message",
            "GetOrderStatus",
            "GetOrderInvoice",
            "GetOrderTracking",
            "request_invoice_view",
            "request_order_status",
            "request_order_invoice",
            "request_order_tracking",
        };

        private static readonly HashSet<string> PoNumberHints = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "GetOrderStatus",
            "GetOrderInvoice",
            "GetOrderTracking",
            "GetCustomerPO",
            "request_invoice_view",
            "request_order_status",
            "request_order_invoice",
            "request_order_tracking",
        };

        public ChatBotFacade(IChatBotService chatBotService, ILLMService llmService,
            ICustomerChatBotCommandHandlerFactory handlerFactory, IMemoryCache cache,
            ICustomerOrderFacade customerOrderFacade)
        {
            _chatBotService = chatBotService;
            _llmService = llmService;
            _handlerFactory = handlerFactory;
            _cache = cache;
            _customerOrderFacade = customerOrderFacade;
        }
        public async Task<int> LogCustomerChatAsync(CustomerChatLogModel model)
        {
            var contract = new MedGyn.MedForce.Service.Contracts.CustomerChatLogContract
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                State = model.State,
                Country = model.Country,
                IsExistingCustomer = model.IsExistingCustomer,
                CustomerId = model.CustomerId
            };
           return await _chatBotService.LogCustomerChatAsync(contract);
        }

        private string BuildLocationContext(string state, string country)
        {
            var parts = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(state)) parts.Add(state);
            if (!string.IsNullOrWhiteSpace(country)) parts.Add(country);

            if (parts.Count == 0) return string.Empty;

            return $" The customer is located in {string.Join(", ", parts)}. Use this location automatically when looking up their sales representative.";
        }

        public async Task<CustomerChatResponseDTO> ProcessMessage(CustomerChatRequestDTO request)
        {
            if (request.ChatLogId <= 0)
                return new CustomerChatResponseDTO { Message = "Invalid session. Please refresh the page and start a new chat." };

            try
            {
                string chatLogCacheKey = $"ChatLog_{request.ChatLogId}";
                string chatLogStateCacheKey = $"ChatLog_{request.ChatLogId}_State";
                string chatLogCountryCacheKey = $"ChatLog_{request.ChatLogId}_Country";

                if (!_cache.TryGetValue(chatLogCacheKey, out int? customerId))
                {
                    var chatLog = await _chatBotService.GetCustomerChatLogAsync(request.ChatLogId);
                    customerId = chatLog?.CustomerId;
                    _cache.Set(chatLogCacheKey, customerId, TimeSpan.FromHours(1));
                    _cache.Set(chatLogStateCacheKey, chatLog?.State, TimeSpan.FromHours(1));
                    _cache.Set(chatLogCountryCacheKey, chatLog?.Country, TimeSpan.FromHours(1));
                }
                request.CustomerId = customerId;

                _cache.TryGetValue(chatLogStateCacheKey, out string customerState);
                _cache.TryGetValue(chatLogCountryCacheKey, out string customerCountry);
                request.Country = customerCountry;

                if (!_cache.TryGetValue(FunctionDeclCacheKey, out string functionJson))
                {
                    functionJson = File.ReadAllText(@".\wwwroot\js\ClaudeFunctionDeclarations.json");
                    _cache.Set(FunctionDeclCacheKey, functionJson);
                }

                string pendingIntentCacheKey = $"ChatLog_{request.ChatLogId}_PendingIntent";

                if (!string.IsNullOrWhiteSpace(request.FunctionHint) && AllowedFunctionHints.Contains(request.FunctionHint))
                {
                    var hintedHandler = _handlerFactory.GetCommandHandler(request.FunctionHint);
                    if (hintedHandler != null)
                    {
                        string hintParams;
                        if (request.FunctionHint.Equals("GetRepersentativeByCountryOrState", StringComparison.OrdinalIgnoreCase))
                            hintParams = JsonConvert.SerializeObject(new { state = customerState, country = customerCountry });
                        else if (PoNumberHints.Contains(request.FunctionHint))
                            hintParams = JsonConvert.SerializeObject(new { po_number = ExtractPoNumber(request.Message) });
                        else
                            hintParams = JsonConvert.SerializeObject(new { message = request.Message });

                        _cache.Remove(pendingIntentCacheKey);
                        return await hintedHandler.HandleAsync(new[] { hintParams }, request);
                    }
                }

                // Free-text follow-up: Claude calls are stateless (no conversation history), so if the
                // user previously asked a PO-related question that Claude only answered conversationally
                // (no function call yet), a bare follow-up like "PO 18991572" would otherwise lose that
                // original intent. Resume it here instead of asking Claude cold with no context.
                if (string.IsNullOrWhiteSpace(request.FunctionHint)
                    && _cache.TryGetValue(pendingIntentCacheKey, out string pendingIntentMessage))
                {
                    var resumedPoNumber = ExtractPoNumber(request.Message);
                    if (!string.IsNullOrWhiteSpace(resumedPoNumber) && resumedPoNumber.Any(char.IsDigit))
                    {
                        var resumedHint = MapMessageToPoFunctionHint(pendingIntentMessage);
                        var resumedHandler = _handlerFactory.GetCommandHandler(resumedHint);
                        if (resumedHandler != null)
                        {
                            _cache.Remove(pendingIntentCacheKey);
                            var resumedParams = JsonConvert.SerializeObject(new { po_number = resumedPoNumber });
                            return await resumedHandler.HandleAsync(new[] { resumedParams }, request);
                        }
                    }
                }

                var locationContext = BuildLocationContext(customerState, customerCountry);
                var systemPrompt = $"You are a helpful MedGyn customer support assistant. Use the available functions to answer the customer's question. Correct any obvious misspellings in medical product names before calling functions. When searching for a product, use only the core descriptive product name and omit packaging or quantity details such as (100/Pack), 50/Box, 25/Case etc.{locationContext}";

                var llmResponse = await _llmService.AgentFunction(
                    request.Message,
                    functionJson,
                    systemPrompt);

                if (llmResponse == null)
                    return new CustomerChatResponseDTO { Message = "I wasn't able to understand your request. Could you please rephrase your question?" };

                if (!string.IsNullOrWhiteSpace(llmResponse.TextResponse))
                {
                    // Claude asked a clarifying question without committing to a function yet.
                    // Remember what the user originally asked so a bare follow-up can be routed correctly.
                    if (ContainsPoIntentKeyword(request.Message))
                        _cache.Set(pendingIntentCacheKey, request.Message, TimeSpan.FromMinutes(10));

                    return new CustomerChatResponseDTO { Message = llmResponse.TextResponse };
                }

                _cache.Remove(pendingIntentCacheKey);
                var handler = _handlerFactory.GetCommandHandler(llmResponse.FunctionName);

                if (handler == null)
                {
                    return new CustomerChatResponseDTO
                    {
                        Message = "I'm not sure how to help with that. Please select an option from the menu or ask about products, orders, or your sales representative."
                    };
                }

                return await handler.HandleAsync(llmResponse.Parameters.ToArray(), request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChatBotFacade.ProcessMessage error: {ex}");
                return new CustomerChatResponseDTO
                {
                    Message = "Something went wrong processing your request. Please try again or contact MedGyn support."
                };
            }
        }

        private static bool ContainsPoIntentKeyword(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return false;
            return new[] { "track", "invoice", "status", "po number", "purchase order" }
                .Any(k => message.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static string MapMessageToPoFunctionHint(string message)
        {
            if (message.IndexOf("track", StringComparison.OrdinalIgnoreCase) >= 0) return "GetOrderTracking";
            if (message.IndexOf("invoice", StringComparison.OrdinalIgnoreCase) >= 0) return "GetOrderInvoice";
            if (message.IndexOf("status", StringComparison.OrdinalIgnoreCase) >= 0) return "GetOrderStatus";
            return "GetCustomerPO";
        }

        private static string ExtractPoNumber(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return message;
            // Strip common "PO Number -", "PO:", "PO#" prefixes and return the rest as-is
            var s = Regex.Replace(message.Trim(), @"(?i)^(p\.?o\.?\s*(number|#|num)?\s*[-:–]?\s*)", "").Trim();
            return s;
        }

        public async Task<byte[]> GetChatbotInvoicePdfAsync(int shipmentId, int chatLogId)
        {
            var chatLog = await _chatBotService.GetCustomerChatLogAsync(chatLogId);
            if (chatLog?.CustomerId == null)
                return null;

            var belongs = await _customerOrderFacade.ShipmentBelongsToCustomerAsync(shipmentId, chatLog.CustomerId.Value);
            if (!belongs)
                return null;

            return _customerOrderFacade.GetInvoice(shipmentId);
        }
    }
}
