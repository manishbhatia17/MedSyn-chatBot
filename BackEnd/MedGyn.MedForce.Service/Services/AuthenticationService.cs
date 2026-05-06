using MedGyn.MedForce.Service.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IAuthenticationService = MedGyn.MedForce.Service.Interfaces.IAuthenticationService;
using MedGyn.MedForce.Data.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISecurityRepository _securityRepository;

		//hack for login
		private List<Claim> _claims;

		public AuthenticationService(IHttpContextAccessor httpContextAccessor, ISecurityRepository securityRepository)
		{
			_httpContextAccessor = httpContextAccessor;
			_securityRepository = securityRepository;
		}

		public async Task Login(UserContract user, bool isRememberMe)
		{
			_claims = GetClaims(user);

			var claimsIdentity = new ClaimsIdentity(
				_claims, CookieAuthenticationDefaults.AuthenticationScheme);

			var authProperties = new AuthenticationProperties
			{
				AllowRefresh = true,
				// Refreshing the authentication session should be allowed.

				ExpiresUtc = DateTimeOffset.Now.AddMinutes(240),
				// The time at which the authentication ticket expires. A 
				// value set here overrides the ExpireTimeSpan option of 
				// CookieAuthenticationOptions set with AddCookie.

				IsPersistent = isRememberMe
				// Whether the authentication session is persisted across 
				// multiple requests. When used with cookies, controls
				// whether the cookie's lifetime is absolute (matching the
				// lifetime of the authentication ticket) or session-based.
			};

			await _httpContextAccessor.HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity),
				authProperties);
		}

		public Claim GetClaim(string claimId)
		{
			var claim = _httpContextAccessor.HttpContext.User.Claims
				.FirstOrDefault(c => c.Type == claimId);

			return claim;
		}

		public List<String> GetClaims()
		{
			return _httpContextAccessor.HttpContext.User.Claims.Where(c => c.Value == "True").Select(c => c.Type).ToList();
		}

		public async Task Logout()
		{
			await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}

		public bool HasAnyClaim(List<int> claimIds)
		{
			foreach (var claimId in claimIds)
			{
				if (HasClaim(claimId))
					return true;
			}
			return false;
		}

		public bool HasClaim(int claimId)
		{
			if(!_httpContextAccessor.HttpContext.User.Claims.Any() && _claims.Any())
				return _claims.Any(c => c.Type == claimId.ToString());
			return _httpContextAccessor.HttpContext.User.Claims
				.Any(c => c.Type == claimId.ToString());
		}

		public string GetName()
		{
			return _httpContextAccessor.HttpContext.User.Identity.Name;
		}

		public int GetUserID()
		{
			var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
			if (claim == null)
			{
				return 0;
			}

			return int.Parse(claim.Value);
		}

		private List<Claim> GetClaims(UserContract user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Name, user.FirstName),
				new Claim(ClaimTypes.Surname, user.LastName),
				new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
			};

			if (user.Role != null)
			{
				claims.Add(new Claim(ClaimTypes.Role, user.Role.RoleId.ToString()));
			}

			var securityKeys = _securityRepository.GetAllSecurityKeysForRole(user.RoleId);
			foreach (var securityKey in securityKeys)
			{
				claims.Add(new Claim(securityKey.SecurityKeyId.ToString(), true.ToString()));
			}

			return claims;
		}

		public string GetHostUrl()
		{
			return _httpContextAccessor.HttpContext.Request.Host.Value;
		}
	}
}
