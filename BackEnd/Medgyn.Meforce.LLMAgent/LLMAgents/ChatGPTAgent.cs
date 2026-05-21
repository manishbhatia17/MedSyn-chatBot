using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OpenAI.Chat;
using Medgyn.Meforce.LLMAgent.Declarations;
using Microsoft.Extensions.Options;
using Medgyn.Meforce.LLMAgent.Configurations;

namespace Medgyn.Meforce.LLMAgent.LLMAgents
{
	public class ChatGPTAgent : ILLMAgent
	{
		HttpClient _httpClient;
		List<ChatMessage> _messages;
		private readonly string _apiKey;
		private readonly string _model;

		public ChatGPTAgent(IHttpClientFactory httpClientFactory, IOptions<OpenAISettings> openAISettings)
		{
			_httpClient = httpClientFactory.CreateClient("ChatGPTClient");
			_apiKey = openAISettings.Value.ApiKey;
			_model = openAISettings.Value.Model ?? "gpt-4o-mini";
			_messages = new List<ChatMessage>();
		}


		public async Task<string> SummarizeContent(string content, string AgentDirections = "")
		{
			if (!string.IsNullOrEmpty(AgentDirections)) 
			{
				_messages.Add(new SystemChatMessage(AgentDirections));
			}

			_messages.Add(new UserChatMessage(content));

			ChatClient chatClient = new ChatClient(_model, _apiKey);

			ChatCompletion chatCompletion = await chatClient.CompleteChatAsync(_messages);

			return chatCompletion.Content[0].Text;
		}

		public async Task<T> AgentFunction<T>(string content, string agentFunctions, string AgentDirections = "")
		{
			try
			{
				string response = string.Empty;
				ChatCompletionOptions options = new ChatCompletionOptions();
				IList<ChatTool> chatTools = CreateFunctions(agentFunctions);

				foreach (var tool in chatTools)
				{
					options.Tools.Add(tool);
				}

				if (!string.IsNullOrEmpty(AgentDirections))
				{
					_messages.Add(new SystemChatMessage(AgentDirections));
				}

				_messages.Add(new UserChatMessage(content));

				ChatClient chatClient = new ChatClient(_model, _apiKey);

				ChatCompletion chatCompletion = await chatClient.CompleteChatAsync(_messages, options);

				if (chatCompletion.ToolCalls.Count > 0)
				{
					for (int i = 0; i < chatCompletion.ToolCalls.Count; i++)
					{
						_messages.Add(new ToolChatMessage(chatCompletion.ToolCalls[i].Id, content));
					}

					var toolCallsList = new List<ChatToolCall>(chatCompletion.ToolCalls);
					return (T)(object)toolCallsList;
				}
				else
				{
					return default(T);
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"ChatGPT AgentFunction failed: {ex.Message}", ex);
			}
		}

		private IList<ChatTool> CreateFunctions(string agentFunctions)
		{
			List<ChatTool> chatTools = new List<ChatTool>();
			List<ChatFunctionRoot> functions = JsonConvert.DeserializeObject<List<ChatFunctionRoot>>(agentFunctions);
			
			foreach (var function in functions)
			{
				ChatTool chatTool = ChatTool.CreateFunctionTool(
					functionName: function.function.name,
					functionDescription: function.function.description,
					functionParameters: BinaryData.FromString(JsonConvert.SerializeObject(function.function.parameters))

				);
				chatTools.Add(chatTool);
			}


			return chatTools;
		}
	}
}
