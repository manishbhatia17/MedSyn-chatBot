using Medgyn.Meforce.LLMAgent.Services;
using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Facade.Handlers
{
    public class GetRepersentativeByStateOrCountryCustomerChatBotCommandHandler
        : ICustomerChatBotCommandHandler
    {
        public CustomerChatBotCommandType CommandType =>
            CustomerChatBotCommandType.GetRepersentativeByCountryOrState;

        private readonly IRepersentativeTerritoryService
            _representativeTerritoryService;

        private readonly ILLMService
            _llmService;

        public GetRepersentativeByStateOrCountryCustomerChatBotCommandHandler(
            IRepersentativeTerritoryService representativeTerritoryService,
            ILLMService llmService)
        {
            _representativeTerritoryService =
                representativeTerritoryService;

            _llmService =
                llmService;
        }

        public async Task<CustomerChatResponseDTO>
            HandleAsync(
                string[] parameters,
                CustomerChatRequestDTO request)
        {
            var dict =
                JsonConvert.DeserializeObject
                    <Dictionary<string, string>>(
                        parameters[0]);



            string state =
    dict.ContainsKey("state")
        ? dict["state"]
        : null;

            string country =
                dict.ContainsKey("country")
                    ? dict["country"]
                    : null;


            var representative =
                await _representativeTerritoryService
                    .GetRepresentativeByLocationAsync(
                        state,country);

            if (representative == null)
            {
                return new CustomerChatResponseDTO
                {
                    FunctionName =
                        CommandType.ToString(),

                    Message =
                        "No sales representative found for the provided location."
                };
            }

            RepresentativeDTO representativeData = new RepresentativeDTO()
            {
                Email = representative.Representative.Email,
                Id = representative.Representative.Id,
                Name = representative.Representative.Name,
                Phone = representative.Representative.Phone
            };

            StringBuilder sb =
                new StringBuilder();

            sb.AppendLine(
                $"Representative Name: {representativeData.Name}");

            sb.AppendLine(
                $"Email: {representativeData.Email}");

            sb.AppendLine(
                $"Phone: {representativeData.Phone}");

            if (!string.IsNullOrEmpty(state))
            {
                sb.AppendLine(
                    $"Location: {state}");
            }

            if (!string.IsNullOrEmpty(country))
            {
                sb.AppendLine(
                    $"Location: {country}");
            }

            string aiResponse =
                await _llmService
                    .SummarizeContent(
                        sb.ToString(),
                        "You are a medical supplies customer support assistant. Explain representative contact information clearly and professionally.");

            return new CustomerChatResponseDTO
            {
                FunctionName =
                    CommandType.ToString(),

                Message =
                    aiResponse,

                Data =
                    representativeData
            };
        }
    }
}