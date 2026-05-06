using Medgyn.Meforce.LLMAgent.LLMAgents;
using Medgyn.Meforce.LLMAgent.Mappers;
using Medgyn.Meforce.LLMAgent.Models;
using System;
using System.Collections.Generic;
using System.Text;
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
			string functionJson = System.IO.File.ReadAllText(@".\wwwroot\js\ChatGPTMCPServerJson.json");
			var response = await _lLMAgent.AgentFunction<List<GeminiFunctionResponse>>("Hello I am in need of a product 123. Thanks!", functionJson);
			if (response.Count > 0 && response[0].Candidates[0].Content.Parts[0].FunctionCall.Name != null)
			{

				return new LLMFunctionServiceContractMapper().GeminiToServiceContract(response[0]);

			}

			return default;
		}
	}
}
