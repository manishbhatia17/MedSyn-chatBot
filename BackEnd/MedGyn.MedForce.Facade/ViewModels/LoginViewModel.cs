using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class LoginViewModel
	{
		public LoginViewModel()
		{
			Errors = new List<string>();
			BuildTimestamp = DateTime.UtcNow.ToLongDateString();
			BuildVersion = string.Empty;
		}

		[EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public bool ForcePasswordReset { get; set; }

		public List<string> Errors { get; set; }
		public bool Success => Errors.Count == 0;
		public string BuildVersion { get; set; }
		public string BuildTimestamp { get; set; }
		public bool RememberMe { get; set; }
	}
}
