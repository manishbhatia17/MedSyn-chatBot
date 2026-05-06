using Medgyn.Meforce.LLMAgent.LLMAgents;
using Medgyn.Meforce.LLMAgent.Mappers;
using Medgyn.Meforce.LLMAgent.Models;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medgyn.Meforce.LLMAgent.Services
{
	public class ChatGPTLLMService : ILLMService
	{
		private readonly ILLMAgent _lLMAgent;
		public ChatGPTLLMService(ILLMAgent llmAgent)
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
			var response = await _lLMAgent.AgentFunction<List<ChatToolCall>>(content, functionJson);
			if (response != null && response.Count > 0 && response[0].FunctionName != null)
			{

				return new LLMFunctionServiceContractMapper().ChatGPTToServiceContract(response[0]);

			}

			return default;
		}
	}
}
