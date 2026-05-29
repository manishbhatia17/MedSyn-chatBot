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

        private readonly IRepersentativeTerritoryService _representativeTerritoryService;

        public GetRepersentativeByStateOrCountryCustomerChatBotCommandHandler(
            IRepersentativeTerritoryService representativeTerritoryService)
        {
            _representativeTerritoryService = representativeTerritoryService;
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

            string location = !string.IsNullOrEmpty(state) ? state : country;

            var sb = new StringBuilder();
            sb.AppendLine($"Your MedGyn sales representative for {location}:");
            sb.AppendLine();
            sb.AppendLine($"**Name:** {representativeData.Name}");
            sb.AppendLine($"**Email:** {representativeData.Email}");
            sb.AppendLine($"**Phone:** {representativeData.Phone}");

            if (!string.IsNullOrWhiteSpace(representative.Territory?.Name))
                sb.AppendLine($"**Territory:** {representative.Territory.Name}");

            sb.AppendLine();
            sb.AppendLine("Feel free to reach out to them directly — they'll be happy to assist you with personalized support!");
            sb.AppendLine();
            sb.AppendLine("Is there anything else I can help you with?");

            return new CustomerChatResponseDTO
            {
                FunctionName = CommandType.ToString(),
                Message = sb.ToString(),
                Data = representativeData
            };
        }
    }
}