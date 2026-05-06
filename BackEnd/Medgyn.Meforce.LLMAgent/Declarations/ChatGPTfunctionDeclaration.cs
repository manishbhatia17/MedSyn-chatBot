using System;
using System.Collections.Generic;
using System.Text;

namespace Medgyn.Meforce.LLMAgent.Declarations
{

		public class ChatFunctionParameterProperty
		{
			public string type { get; set; }
			public string description { get; set; }
		}

		public class ChatFunctionParameters
		{
			public string type { get; set; }
			public Dictionary<string, ChatFunctionParameterProperty> properties { get; set; }
			public List<string> required { get; set; }
		}

		public class ChatFunction
		{
			public string name { get; set; }
			public string description { get; set; }
			public ChatFunctionParameters parameters { get; set; }
		}

		public class ChatFunctionRoot
		{
			public string type { get; set; }
			public ChatFunction @function { get; set; }
		}
	
}
