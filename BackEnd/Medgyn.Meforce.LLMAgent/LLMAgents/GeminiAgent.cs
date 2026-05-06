using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Medgyn.Meforce.LLMAgent.Declarations;

namespace Medgyn.Meforce.LLMAgent.LLMAgents
{
	public class GeminiAgent : ILLMAgent
	{
		HttpClient _httpClient;
		private const string _apiKey = "AIzaSyCR2k5IlPmSL_uHBoyW3BMk76LA8pdWqWQ";
		public GeminiAgent(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("GeminiClient");
		}

		public async Task<T> AgentFunction<T>(string content, string agentFunctions, string AgentDirections = "")
		{
			var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key={_apiKey}";

			var geminiPayload = new Dictionary<string, object>();

			if (!string.IsNullOrWhiteSpace(AgentDirections))
			{
				geminiPayload["system_instruction"] = new
				{
					parts = new[]
					{
							new { text = AgentDirections } // Example: "You are a cat. Your name is Neko."
						}
				};
			}

			geminiPayload["tools"] = new[]
			{
				new
				{
					functionDeclarations = JsonConvert.DeserializeObject<List<FunctionDeclaration>>(agentFunctions)
				}
			};

			geminiPayload["contents"] = new[]
			{
				new
				{
					parts = new[]
					{
						new
						{
							text = $"Review this content and summarize: '{content}'"
						}
					}
				}
			};

			StringContent geminiPayloadString = new StringContent(
				JsonConvert.SerializeObject(geminiPayload),
				Encoding.UTF8,
				"application/json"
			);
			var response = await _httpClient.PostAsync(endpoint, geminiPayloadString);

			if (!response.IsSuccessStatusCode)
			{
				string err = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error calling Gemini API: {response.ReasonPhrase}. {err}");
			}

			var result = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<T>(result);
		}

		public async Task<string> SummarizeContent(string content, string AgentDirections = "")
		{

			var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key={_apiKey}";

			var geminiPayload = new Dictionary<string, object>();

			if (!string.IsNullOrWhiteSpace(AgentDirections))
			{
					geminiPayload["system_instruction"] = new
					{
						parts = new[]
						{
							new { text = AgentDirections } // Example: "You are a cat. Your name is Neko."
						}
					};
			}

			geminiPayload["contents"] = new[]
			{
				new
				{
					parts = new[]
					{
						new
						{
							text = $"{content}"
						}
					}
				}
			};

			StringContent geminiPayloadString = new StringContent(
				JsonConvert.SerializeObject(geminiPayload),
				Encoding.UTF8,
				"application/json"
			);
			var response = await _httpClient.PostAsync(endpoint, geminiPayloadString);

			if(!response.IsSuccessStatusCode)
			{
				string err = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error calling Gemini API: {response.ReasonPhrase}. {err}");
			}

			var result = await response.Content.ReadAsStringAsync();

			return result;
		}

	}
}
