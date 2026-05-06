using Medgyn.Meforce.LLMAgent.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medgyn.Meforce.LLMAgent.Services
{
	public interface ILLMService
	{
		Task<string> SummarizeContent(string content, string agentDirections = "");
		Task<LLMFunctionServiceContract> AgentFunction(string content, string agentFunctions, string agentDirections = "");
	}
}
