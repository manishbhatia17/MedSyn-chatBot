using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
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

        private readonly IRepersentativeTerritoryService
            _representativeTerritoryService;

        private readonly IChatBotService
            _chatBotService;

        private readonly IEmailService
            _emailService;

        public LeaveMessageForMedGynCustomerChatBotCommandHandler(
            IRepersentativeTerritoryService representativeTerritoryService,
            IChatBotService chatBotService,
            IEmailService emailService)
        {
            _representativeTerritoryService =
                representativeTerritoryService;

            _chatBotService =
                chatBotService;

            _emailService =
                emailService;
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

                RepresentativeTerritory representativeTerritory =
                    await _representativeTerritoryService
                        .GetRepresentativeByCustomerChatLogId(
                            request.ChatLogId);

                Representative representative =
                    representativeTerritory?
                        .Representative;

                template =
                    template.Replace(
                        "@CustomerName",
                        customer.Name);

                template =
                    template.Replace(
                        "@CustomerEmail",
                        customer.Email);

                template =
                    template.Replace(
                        "@State",
                        customer.State);

                template =
                    template.Replace(
                        "@Country",
                        customer.Country);

                template =
                    template.Replace(
                        "@CustomerMessage",
                        customerMessage);

                string repEmail =
                    representative?
                        .Email;

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
                    FunctionName =
                        CommandType.ToString(),

                    Message =
                        "Thank you. Your message has been forwarded to the MedGyn team."
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