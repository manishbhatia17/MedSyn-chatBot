
namespace MedGyn.MedForce.Common.SharedModels
{ 
	public class Error
	{
		public string Message { get; set; }
		public string ExceptionMessage { get; set; }
		public string ExceptionType { get; set; }
		public string StackTrace { get; set; }
	}
}
