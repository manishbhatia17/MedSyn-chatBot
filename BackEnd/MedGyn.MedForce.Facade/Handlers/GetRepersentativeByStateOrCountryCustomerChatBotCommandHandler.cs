using MedGyn.MedForce.Facade.DTOs;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Facade.Interfaces;
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

        private readonly ISalesTerritoryFacade _salesTerritoryFacade;

        public GetRepersentativeByStateOrCountryCustomerChatBotCommandHandler(
            ISalesTerritoryFacade salesTerritoryFacade)
        {
            _salesTerritoryFacade = salesTerritoryFacade;
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
                await _salesTerritoryFacade
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

            string location = !string.IsNullOrEmpty(state) ? state : country;

            var sb = new StringBuilder();
            sb.AppendLine($"Your MedGyn sales representative for {location}:");
            sb.AppendLine();
            sb.AppendLine($"**Name:** {representative.Name}");
            sb.AppendLine($"**Email:** {representative.Email}");
            sb.AppendLine($"**Phone:** {representative.Phone}");

            if (!string.IsNullOrWhiteSpace(representative.TerritoryName))
                sb.AppendLine($"**Territory:** {representative.TerritoryName}");

            sb.AppendLine();
            sb.AppendLine("Feel free to reach out to them directly — they'll be happy to assist you with personalized support!");
            sb.AppendLine();
            sb.AppendLine("Is there anything else I can help you with?");

            return new CustomerChatResponseDTO
            {
                FunctionName = CommandType.ToString(),
                Message = sb.ToString(),
                Data = representative
            };
        }
    }
}