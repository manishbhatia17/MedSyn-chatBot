using Medgyn.Meforce.LLMAgent.Models;
using Newtonsoft.Json;
using OpenAI.Chat;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medgyn.Meforce.LLMAgent.Mappers
{
	public class LLMFunctionServiceContractMapper
	{
		public LLMFunctionServiceContract ChatGPTToServiceContract(ChatToolCall toolCall)
		{
			return new LLMFunctionServiceContract()
			{
				FunctionName = toolCall.FunctionName,
				Parameters = new List<string> { toolCall.FunctionArguments.ToString() }
			};
		}

		public LLMFunctionServiceContract GeminiToServiceContract(GeminiFunctionResponse geminiFunctionResponse)
		{
			var functionCall = geminiFunctionResponse.Candidates
				.SelectMany(c => c.Content.Parts)
				.First(p => p.FunctionCall != null)
				.FunctionCall;

			return new LLMFunctionServiceContract
			{
				FunctionName = functionCall.Name,
				Parameters = new List<string> { JsonConvert.SerializeObject(functionCall.Args) }
			};
		}

		public LLMFunctionServiceContract ClaudeToServiceContract(ClaudeMessageResponse response)
		{
			var toolUseBlock = response.Content.First(c => c.Type == "tool_use");

			return new LLMFunctionServiceContract
			{
				FunctionName = toolUseBlock.Name,
				Parameters = new List<string> { JsonConvert.SerializeObject(toolUseBlock.Input) }
			};
		}
	}
}
