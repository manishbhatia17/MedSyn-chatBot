using System;
using System.Collections.Generic;
using System.Text;

namespace Medgyn.Meforce.LLMAgent.Models
{
	public class LLMFunctionServiceContract
	{
		public string FunctionName { get; set; }
		public List<string> Parameters { get; set; }
	}
}
