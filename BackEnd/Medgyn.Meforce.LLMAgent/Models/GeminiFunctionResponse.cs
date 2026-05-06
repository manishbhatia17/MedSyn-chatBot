using System;
using System.Collections.Generic;
using System.Text;

namespace Medgyn.Meforce.LLMAgent.Models
{
	public class GeminiFunctionResponse
	{
		public List<Candidate> Candidates { get; set; }
		public UsageMetadata UsageMetadata { get; set; }
		public string ModelVersion { get; set; }
		public string ResponseId { get; set; }
	}

	public class Candidate
	{
		public Content Content { get; set; }
		public string FinishReason { get; set; }
		public int Index { get; set; }
	}

	public class Content
	{
		public List<Part> Parts { get; set; }
		public string Role { get; set; }
	}

	public class Part
	{
		public FunctionCall FunctionCall { get; set; }
		public string ThoughtSignature { get; set; }
	}

	public class FunctionCall
	{
		public string Name { get; set; }
		public Dictionary<string, string> Args { get; set; } // or use a typed model if args are predictable
	}

	public class UsageMetadata
	{
		public int PromptTokenCount { get; set; }
		public int CandidatesTokenCount { get; set; }
		public int TotalTokenCount { get; set; }
		public List<PromptTokensDetail> PromptTokensDetails { get; set; }
		public int ThoughtsTokenCount { get; set; }
	}

	public class PromptTokensDetail
	{
		public string Modality { get; set; }
		public int TokenCount { get; set; }
	}
}
