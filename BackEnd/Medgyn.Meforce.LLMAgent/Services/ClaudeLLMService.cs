using System.Linq;
using System.Threading.Tasks;
using Medgyn.Meforce.LLMAgent.LLMAgents;
using Medgyn.Meforce.LLMAgent.Mappers;
using Medgyn.Meforce.LLMAgent.Models;

namespace Medgyn.Meforce.LLMAgent.Services
{
    public class ClaudeLLMService : ILLMService
    {
        private readonly ILLMAgent _llmAgent;

        public ClaudeLLMService(ILLMAgent llmAgent)
        {
            _llmAgent = llmAgent;
        }

        public async Task<string> SummarizeContent(string content, string agentDirections = "")
        {
            return await _llmAgent.SummarizeContent(content, agentDirections);
        }

        public async Task<LLMFunctionServiceContract> AgentFunction(string content, string agentFunctions, string agentDirections = "")
        {
            var response = await _llmAgent.AgentFunction<ClaudeMessageResponse>(content, agentFunctions, agentDirections);

            var toolUseBlock = response?.Content?.FirstOrDefault(c => c.Type == "tool_use");

            if (toolUseBlock?.Name != null)
                return new LLMFunctionServiceContractMapper().ClaudeToServiceContract(response);

            var textBlock = response?.Content?.FirstOrDefault(c => c.Type == "text");
            if (!string.IsNullOrWhiteSpace(textBlock?.Text))
                return new LLMFunctionServiceContract { TextResponse = textBlock.Text };

            return default;
        }
    }
}
