namespace MedGyn.MedForce.Facade.ViewModels
{
	public class StatusViewModel
	{
		public string DatabaseVersion { get; set; }
		public string CurrentVersion { get; set; }
		public bool IsDatabaseConnected { get; set; }
		public bool? IsUpdatedSuccessfully { get; set; }
		public string UpdatePassword { get; set; }
		public bool? IsPublishedSuccessfully { get; set; }
		public string PublishPassword { get; set; }
	}
}
