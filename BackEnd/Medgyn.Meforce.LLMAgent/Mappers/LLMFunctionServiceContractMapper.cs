using Medgyn.Meforce.LLMAgent.Models;
using OpenAI.Chat;
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
				Parameters = new List<string>(toolCall.FunctionArguments.ToString().Split(','))
			};
		}

		public LLMFunctionServiceContract GeminiToServiceContract(GeminiFunctionResponse geminiFunctionResponse)
		{
			return new LLMFunctionServiceContract
			{
				FunctionName = "GetProductByName",
				Parameters = new List<string> { "product_name" }
			};
		}
	}
}
