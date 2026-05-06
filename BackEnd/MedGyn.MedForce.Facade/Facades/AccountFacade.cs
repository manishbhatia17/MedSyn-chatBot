using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Helpers;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace MedGyn.MedForce.Facade.Facades
{
	public class AccountFacade : IAccountFacade
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IEmailService _emailService;
		private readonly ISecurityService _securityService;
		private readonly IUserService _userService;
		private readonly IAuthenticationService _authenticationService;

		public AccountFacade(IHttpContextAccessor httpContextAccessor, IEmailService emailService, ISecurityService securityService,
			IUserService userService, IAuthenticationService authenticationService)
		{
			_httpContextAccessor = httpContextAccessor;
			_emailService = emailService;
			_securityService = securityService;
			_userService = userService;
			_authenticationService = authenticationService;
		}

		public bool IsAuthorized(string url, int userId)
		{
			var matchGroupName = "page";
			var match = Regex.Match(url, $@"/GeneratedPage/(?<{matchGroupName}>[-\w\d]+)/.*", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			if (match.Success)
			{
				return _securityService.CanUserAccessPage(userId, match.Groups[matchGroupName].Value);
			}

			return true;
		}

		public LoginViewModel Login(LoginViewModel model)
		{
			var user = _userService.GetUser(model.Email);
			if (user == null || user.IsDeleted)
			{
				model.Errors.Add("Email or Password is invalid.");
				return model;
			}

			if (user.Email == null || user.UserId < 1)
			{
				model.Errors.Add("Email or Password is invalid.");
				return model;
			}

            if (!PasswordHelper.VerifyPasswordHash(model.Password, user.Password, user.PasswordSalt))
            {
                model.Errors.Add("Email or Password is invalid.");
                return model;
            }

            if (user.ResetPasswordOn.HasValue && DateTime.Compare(DateTime.UtcNow, user.ResetPasswordOn.Value.AddDays(7)) > 0)
			{
				model.Errors.Add("Reset password has expired. Please reset your password again to login.");
				return model;
			}

			if (user.ForcePasswordReset)
			{
				model.ForcePasswordReset = true;
				return model;
			}

			_authenticationService.Login(user, model.RememberMe);

			return model;
		}

		public void Logout()
		{
			_authenticationService.Logout();
		}

		public ResetPasswordViewModel ResetPassword(ResetPasswordViewModel model)
		{
			var user = _userService.GetUser(model.Email);
			if (user != null && !user.IsDeleted)
			{
				var newPassword = PasswordHelper.GenerateRandomPassword(10);
				PasswordHelper.HashPassword(newPassword, out var hashedPassword, out var passwordSalt);

				user.ForcePasswordReset = true;
				user.Password = hashedPassword;
				user.PasswordSalt = passwordSalt;
				user.ResetPasswordOn = DateTime.UtcNow;

				_userService.ResetPassword(user);

				var baseUrl = _httpContextAccessor.HttpContext?.Request.GetDisplayUrl().Replace(_httpContextAccessor.HttpContext.Request.Path, "");

				var subject = "Reset Password";
				var bodyTemplate = _emailService.GetEmailTemplate("ResetPassword.html");
				var body = bodyTemplate.Replace("@Link", baseUrl).Replace("@Email", user.Email).Replace("@Password", newPassword);

				_emailService.SendEmail(model.Email, subject, body);
			}

			model.Email = null;
			return model;
		}

		public ChangePasswordViewModel CheckChangePassword(string email)
		{
			var model = new ChangePasswordViewModel();
			if (email == null)
			{
				return model;
			}

			var user = _userService.GetUser(email);
			if (user == null || !user.ForcePasswordReset)
			{
				return model;
			}

			model.Email = user.Email;
			return model;
		}

		public async Task<ChangePasswordViewModel> ChangePassword(ChangePasswordViewModel model)
		{
			ValidateChangePasswordViewModel(model);

			var user = _userService.GetUser(model.Email);
			if (user == null || !user.ForcePasswordReset)
			{
				model.Errors.Add("Email is invalid.");
			}

			if (!model.Success)
			{
				return model;
			}

			PasswordHelper.HashPassword(model.Password, out var hashedPassword, out var passwordSalt);

			user.ForcePasswordReset = false;
			user.Password = hashedPassword;
			user.PasswordSalt = passwordSalt;
			user.ResetPasswordOn = null;

			_userService.ResetPassword(user);
			await _authenticationService.Login(user, false);

			return model;
		}

		#region private functions

		private void ValidateChangePasswordViewModel(ChangePasswordViewModel model)
		{
			if (string.IsNullOrEmpty(model.Email))
			{
				model.Errors.Add("Email is required.");
			}
			else
			{
				var emailValidator = new EmailAddressAttribute();
				if (!emailValidator.IsValid(model.Email))
				{
					model.Errors.Add("Email is not in the correct format.");
				}
			}

			if (string.IsNullOrEmpty(model.Password))
			{
				model.Errors.Add("Password is required.");
			}
			else
			{
				if (model.Password.Length < 8)
				{
					model.Errors.Add("Password must contain at least 8 characters.");
				}

				var upperRegex = new Regex("[A-Z]+");
				var numberRegex = new Regex("[0-9]+");
				var specialCharacterRegex = new Regex("[!@#\\$%\\^&*\\(\\)_+=?><~]");

				if (!upperRegex.IsMatch(model.Password))
				{
					model.Errors.Add("Password must contain a capital letter.");
				}

				if (!numberRegex.IsMatch(model.Password))
				{
					model.Errors.Add("Password must contain a number.");

				}

				if (!specialCharacterRegex.IsMatch(model.Password))
				{
					model.Errors.Add("Password must contain a special character.");

				}
			}

			if (model.ConfirmPassword != model.Password)
			{
				model.Errors.Add("Passwords do not match.");
			}
		}

		#endregion
	}
}
