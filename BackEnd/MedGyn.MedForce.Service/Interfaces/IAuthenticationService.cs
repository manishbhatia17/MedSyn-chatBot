using MedGyn.MedForce.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IAuthenticationService
	{
		Task Login(UserContract user, bool isRememberMe);
		Task Logout();
		Claim GetClaim(string claimID);
		List<String> GetClaims();
		bool HasAnyClaim(List<int> claimIds);
		bool HasClaim(int claimID);
		string GetName();
		int GetUserID();
		string GetHostUrl();

    }
}
