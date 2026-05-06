using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace MedGyn.MedForce.Service.Services
{
	public class UserService : IUserService
	{
		private readonly ISecurityRepository _securityRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMemoryCache _memoryCache;

		private const string userCache = "UserCache";
		private const string roleCache = "RoleCache";

		public UserService(IUserRepository userRepository, ISecurityRepository securityRepository, IMemoryCache MemoryCache)
		{
			_userRepository = userRepository;
			_securityRepository = securityRepository;
			_memoryCache = MemoryCache;
		}

		public List<UserContract> GetAllUsers()
		{
            var users = new List<UserContract>();

			if (!_memoryCache.TryGetValue(userCache, out users))
			{
				users = new List<UserContract>();
				var dbUsers = _userRepository.GetAllUsers();

				foreach (var user in dbUsers)
				{
					users.Add(new UserContract(user));
				}

				//add users to cache for 8 hours
				_memoryCache.Set(userCache, users, DateTime.Now.AddHours(8));
			}
			return users;
		}

		public async ValueTask<List<UserContract>> GetAllUsersAsync()
		{
			Task<List<UserContract>> usersTask = Task.Run(() => GetAllUsers());

			return await usersTask;
		}

		public IList<dynamic> GetUserList(string search, string sortCol, bool sortAsc)
		{
			IList<dynamic> users = _userRepository.GetUserList(search, sortCol, sortAsc);
			return users.Where(w => w.IsHidden == false).ToList();
		}

		public IList<UserContract> GetUsersWithKey(params SecurityKeyEnum[] keys)
		{
			var roleSecKeys = _securityRepository.GetAllRoleSecurityKeys();
			var authorizedRoles = roleSecKeys.Join(
				keys,
				roleSecKey => roleSecKey.SecurityKeyId,
				k => (int)k,
				(r, k) => {
					return r.RoleId;
				}
			).Distinct().ToArray();
			if(authorizedRoles.Length == 0)
			{
				return new List<UserContract>();
			}

			var users = _userRepository.GetUsersWithRoleIn(authorizedRoles);

			users = users.Where(w => w.IsHidden == false).ToList();

			return users.Select(u => new UserContract(u)).ToList();
		}

		public List<RoleContract> GetAllRoles()
		{
            var roles = new List<RoleContract>();

			if (!_memoryCache.TryGetValue(roleCache, out roles))
			{
				roles = new List<RoleContract>();
				var dbRoles = _userRepository.GetAllRoles();


				foreach (var role in dbRoles)
				{
					roles.Add(new RoleContract(role));
				}

				//cache role for 8 hours
				_memoryCache.Set(roleCache, roles, DateTime.Now.AddHours(8));
			}
			return roles;
		}

		public Dictionary<int, UserContract> GetUsersDictionary()
		{
			var dict = new Dictionary<int, UserContract>();

			var dbUsers = _userRepository.GetAllUsers();
			dbUsers = dbUsers.Where(w => w.IsHidden == false).ToList();
			foreach (var dbUser in dbUsers)
			{
				dict.Add(dbUser.UserId, new UserContract(dbUser));
			}

			return dict;
		}

		public UserContract GetUser(int userId)
		{
			var user = _userRepository.GetUser(userId);
			if (user == null)
			{
				return null;
			}
			user.RoleId = user.Role.RoleId;

			return new UserContract(user);
		}

		public UserContract GetUser(string email)
		{
			var user = _userRepository.GetUser(email);
			if (user == null)
			{
				return null;
			}
			user.RoleId = user.Role.RoleId;

			return new UserContract(user);
		}

		public UserContract SaveUser(UserContract updatedUser)
		{
			var curUser = _userRepository.GetUser(updatedUser.UserId);
			var userModel = updatedUser.ToModel(curUser);

			_userRepository.SaveUser(userModel);

			//clear user cache then get users
			_memoryCache.Remove(userCache);
			GetAllUsers();

			return new UserContract(userModel);
		}

		public UserContract DeleteUser(UserContract updatedUser)
		{
			var curUser = _userRepository.GetUser(updatedUser.UserId);
			var userModel = updatedUser.ToModel(curUser);

			_userRepository.DeleteUser(userModel);

            //clear user cache then get users
            _memoryCache.Remove(userCache);
            GetAllUsers();

            return new UserContract(userModel);
		}

		public void ResetPassword(UserContract user)
		{
			_userRepository.ResetPassword(user.UserId, user.ForcePasswordReset, user.Password, user.PasswordSalt, user.ResetPasswordOn);
		}

		public bool ValidateEmail(int userID, string email)
		{
			return _userRepository.ValidateEmail(userID, email);
		}
	}
}
