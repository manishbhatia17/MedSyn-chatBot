using System;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class SaveResults
	{
		public SaveResults() { }
		public SaveResults(string errorMsg)
		{
			ErrorMessage = errorMsg;
		}
		public SaveResults(Exception e)
		{
			ErrorMessage = e.Message;
			FullError = e.ToString();
		}

		public bool Success => ErrorMessage.IsNullOrEmpty();
		public string ErrorMessage { get; set; }
		public string FullError { get; set; }
		public object Payload { get; set; }
	}
}
