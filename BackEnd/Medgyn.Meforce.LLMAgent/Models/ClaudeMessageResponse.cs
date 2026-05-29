using System.Collections.Generic;
using Newtonsoft.Json;

namespace Medgyn.Meforce.LLMAgent.Models
{
    public class ClaudeMessageResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public List<ClaudeContentBlock> Content { get; set; }

        [JsonProperty("stop_reason")]
        public string StopReason { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }
    }

    public class ClaudeContentBlock
    {
        [JsonProperty("type")]
        public string Type { get; set; }  // "text" or "tool_use"

        [JsonProperty("text")]
        public string Text { get; set; }  // when type = "text"

        [JsonProperty("id")]
        public string Id { get; set; }    // when type = "tool_use"

        [JsonProperty("name")]
        public string Name { get; set; }  // when type = "tool_use"

        [JsonProperty("input")]
        public Dictionary<string, object> Input { get; set; }  // when type = "tool_use"
    }
}
