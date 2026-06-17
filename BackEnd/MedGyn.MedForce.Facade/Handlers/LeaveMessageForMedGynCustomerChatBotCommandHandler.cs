using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class LeaveMessageForMedGynCustomerChatBotCommandHandler
        : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType =>
            CustomerChatBotCommandType.LeaveMessageForMedGyn;

        private readonly ISalesTerritoryFacade _salesTerritoryFacade;
        private readonly IChatBotService _chatBotService;
        private readonly IEmailService _emailService;

        public LeaveMessageForMedGynCustomerChatBotCommandHandler(
            ISalesTerritoryFacade salesTerritoryFacade,
            IChatBotService chatBotService,
            IEmailService emailService)
        {
            _salesTerritoryFacade = salesTerritoryFacade;
            _chatBotService = chatBotService;
            _emailService = emailService;
        }

        public async Task<CustomerChatResponseDTO>
            HandleAsync(
                string[] parameters,
                CustomerChatRequestDTO request)
        {
            try
            {
                var dict =
                    JsonConvert.DeserializeObject<
                        Dictionary<string, string>>(
                            parameters[0]);

                string template =
    _emailService
        .GetEmailTemplate(
            "CustomerChatInquiry.html");

                string customerMessage =
                    dict["message"];

                var customer =
                    await _chatBotService
                        .GetCustomerChatLogAsync(
                            request.ChatLogId);

                if (customer == null)
                {
                    return new CustomerChatResponseDTO
                    {
                        FunctionName =
                            CommandType.ToString(),

                        Message =
                            "Customer information could not be found."
                    };
                }

                var representative = await _salesTerritoryFacade
                    .GetRepresentativeByLocationAsync(customer.State, customer.Country);

                template = template.Replace("@CustomerName",    customer.Name);
                template = template.Replace("@CustomerEmail",   customer.Email);
                template = template.Replace("@CustomerPhone",   customer.PhoneNumber ?? string.Empty);
                template = template.Replace("@State",           customer.State);
                template = template.Replace("@Country",         customer.Country);
                template = template.Replace("@CustomerMessage", customerMessage);

                string repEmail = representative?.Email;

                if (!string.IsNullOrWhiteSpace(repEmail))
                {
                    _emailService.SendEmail(
                        repEmail,
                        "Customer Chat Inquiry",
                        template,
                        "info@medgyn.com");
                }
                else
                {
                    _emailService.SendEmail(
                        "info@medgyn.com",
                        "Customer Chat Inquiry",
                        template);
                }

                return new CustomerChatResponseDTO
                {
                    FunctionName = CommandType.ToString(),
                    Message = "Thank you. Your message has been forwarded to the MedGyn team.",
                    Data = new { sent = true }
                };
            }
            catch (Exception)
            {
                return new CustomerChatResponseDTO
                {
                    FunctionName =
                        CommandType.ToString(),

                    Message =
                        "Sorry, there was an issue sending your message. Please try again later."
                };
            }
        }
    }
}