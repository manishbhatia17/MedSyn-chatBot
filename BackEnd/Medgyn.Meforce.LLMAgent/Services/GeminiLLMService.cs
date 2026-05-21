using Medgyn.Meforce.LLMAgent.LLMAgents;
using Medgyn.Meforce.LLMAgent.Mappers;
using Medgyn.Meforce.LLMAgent.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Medgyn.Meforce.LLMAgent.Services
{
	public class GeminiLLMService : ILLMService
	{
		private readonly ILLMAgent _lLMAgent;

		public GeminiLLMService(ILLMAgent llmAgent)
		{
			_lLMAgent = llmAgent;
		}

		public async Task<string> SummarizeContent(string content, string agentDirections = "")
		{
			return await _lLMAgent.SummarizeContent(content, agentDirections);
		}

		public async Task<LLMFunctionServiceContract> AgentFunction(string content, string agentFunctions, string agentDirections = "")
		{
			string functionJson = File.ReadAllText(@".\wwwroot\js\GeminiFunctionDeclarations.json");

			var response = await _lLMAgent.AgentFunction<GeminiFunctionResponse>(content, functionJson, agentDirections);

			var functionCall = response?.Candidates?
				.FirstOrDefault()?.Content?.Parts?
				.FirstOrDefault(p => p.FunctionCall != null)?.FunctionCall;

			if (functionCall?.Name != null)
			{
				return new LLMFunctionServiceContractMapper().GeminiToServiceContract(response);
			}

			return default;
		}
	}
}
