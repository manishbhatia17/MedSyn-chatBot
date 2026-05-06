using System;
using System.Threading.Tasks;

namespace Medgyn.Meforce.LLMAgent.LLMAgents
{
	public interface ILLMAgent
	{
		Task<string> SummarizeContent(string content, string AgentDirections = "");
		Task<T> AgentFunction<T>(string content, string agentFunctions, string AgentDirections = "");
	}
}
