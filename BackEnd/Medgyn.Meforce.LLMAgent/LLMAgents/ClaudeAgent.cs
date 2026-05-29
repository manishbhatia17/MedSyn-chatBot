using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Medgyn.Meforce.LLMAgent.Configurations;
using Medgyn.Meforce.LLMAgent.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Medgyn.Meforce.LLMAgent.LLMAgents
{
    public class ClaudeAgent : ILLMAgent
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private const string _endpoint = "https://api.anthropic.com/v1/messages";
        private const string _anthropicVersion = "2023-06-01";

        public ClaudeAgent(IHttpClientFactory httpClientFactory, IOptions<ClaudeSettings> claudeSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ClaudeClient");
            _apiKey = claudeSettings.Value.ApiKey;
            _model = claudeSettings.Value.Model ?? "claude-3-5-haiku-20241022";
        }

        public async Task<string> SummarizeContent(string content, string AgentDirections = "")
        {
            var requestBody = new Dictionary<string, object>
            {
                ["model"] = _model,
                ["max_tokens"] = 1024,
                ["messages"] = new[] { new { role = "user", content = content } }
            };

            if (!string.IsNullOrWhiteSpace(AgentDirections))
                requestBody["system"] = AgentDirections;

            var response = await SendRequest(requestBody);
            var claudeResponse = JsonConvert.DeserializeObject<ClaudeMessageResponse>(response);

            var textBlock = claudeResponse?.Content?.FirstOrDefault(c => c.Type == "text");
            return textBlock?.Text ?? "Unable to generate a response.";
        }

        public async Task<T> AgentFunction<T>(string content, string agentFunctions, string AgentDirections = "")
        {
            var tools = JsonConvert.DeserializeObject<List<JObject>>(agentFunctions);

            var requestBody = new Dictionary<string, object>
            {
                ["model"] = _model,
                ["max_tokens"] = 1024,
                ["tools"] = tools,
                ["messages"] = new[] { new { role = "user", content = content } }
            };

            if (!string.IsNullOrWhiteSpace(AgentDirections))
                requestBody["system"] = AgentDirections;

            var result = await SendRequest(requestBody);
            return JsonConvert.DeserializeObject<T>(result);
        }

        private async Task<string> SendRequest(Dictionary<string, object> requestBody)
        {
            int maxRetries = 5;
            int delayMs = 500;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _endpoint);
                request.Headers.Add("x-api-key", _apiKey);
                request.Headers.Add("anthropic-version", _anthropicVersion);
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();

                string err = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                // Retry on overloaded (529), service unavailable (503), or rate limit (429)
                bool shouldRetry = statusCode == 529 || statusCode == 503 || statusCode == 429
                                   || err.Contains("overloaded");

                if (shouldRetry && attempt < maxRetries)
                {
                    await Task.Delay(delayMs * attempt);
                    continue;
                }

                throw new InvalidOperationException($"Claude API failed: {response.ReasonPhrase}. {err}");
            }

            throw new InvalidOperationException("Claude API is currently overloaded. Please try again in a moment.");
        }
    }
}
