using System;
using System.Collections.Generic;
using System.Text;

namespace Medgyn.Meforce.LLMAgent.Models
{
	public class ChatGPTFunctionResponse
	{
		public string Type { get; set; }
		public string Id { get; set; }
		public string CallId { get; set; }
		public string Name { get; set; }
		public string Arguments { get; set; }
	}
}
