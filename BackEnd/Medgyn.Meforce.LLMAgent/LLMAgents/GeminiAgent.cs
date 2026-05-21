using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Medgyn.Meforce.LLMAgent.Declarations;
using Medgyn.Meforce.LLMAgent.Models;

namespace Medgyn.Meforce.LLMAgent.LLMAgents
{
	public class GeminiAgent : ILLMAgent
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		private readonly string _model;

		public GeminiAgent(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("GeminiClient");
			_apiKey = string.Empty;
			_model = "gemini-2.0-flash-lite";
		}

		public async Task<string> SummarizeContent(string content, string AgentDirections = "")
		{
			var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

			var geminiPayload = new Dictionary<string, object>();

			if (!string.IsNullOrWhiteSpace(AgentDirections))
			{
				geminiPayload["system_instruction"] = new
				{
					parts = new[] { new { text = AgentDirections } }
				};
			}

			geminiPayload["contents"] = new[]
			{
				new { parts = new[] { new { text = content } } }
			};

			var response = await _httpClient.PostAsync(
				endpoint,
				new StringContent(JsonConvert.SerializeObject(geminiPayload), Encoding.UTF8, "application/json")
			);

			if (!response.IsSuccessStatusCode)
			{
				string err = await response.Content.ReadAsStringAsync();
				throw new InvalidOperationException($"Gemini SummarizeContent failed: {response.ReasonPhrase}. {err}");
			}

			var result = await response.Content.ReadAsStringAsync();
			var geminiResponse = JsonConvert.DeserializeObject<GeminiFunctionResponse>(result);
			var textPart = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault(p => !string.IsNullOrEmpty(p.Text));
			return textPart?.Text ?? "Unable to generate a response.";
		}

		public async Task<T> AgentFunction<T>(string content, string agentFunctions, string AgentDirections = "")
		{
			var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

			var geminiPayload = new Dictionary<string, object>();

			if (!string.IsNullOrWhiteSpace(AgentDirections))
			{
				geminiPayload["system_instruction"] = new
				{
					parts = new[] { new { text = AgentDirections } }
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
				new { parts = new[] { new { text = content } } }
			};

			var response = await _httpClient.PostAsync(
				endpoint,
				new StringContent(JsonConvert.SerializeObject(geminiPayload), Encoding.UTF8, "application/json")
			);

			if (!response.IsSuccessStatusCode)
			{
				string err = await response.Content.ReadAsStringAsync();
				throw new InvalidOperationException($"Gemini AgentFunction failed: {response.ReasonPhrase}. {err}");
			}

			var result = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(result);
		}
	}
}
