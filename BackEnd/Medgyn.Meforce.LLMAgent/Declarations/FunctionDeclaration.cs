using System;
using System.Collections.Generic;
using System.Text;

namespace Medgyn.Meforce.LLMAgent.Declarations
{
	public class FunctionDeclaration
	{
		public string name { get; set; }
		public string description { get; set; }
		public Parameters parameters { get; set; }
	}

	public class Parameters
	{
		public string type { get; set; }
		public Dictionary<string, Property> properties { get; set; }
		public List<string> required { get; set; }
	}

	public class Property
	{
		public string type { get; set; }
		public string description { get; set; }

		// Optional nested type support (e.g., arrays)
		public Items items { get; set; }
	}

	public class Items
	{
		public string type { get; set; }
	}

}
