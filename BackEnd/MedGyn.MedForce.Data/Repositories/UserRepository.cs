using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Data.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly ConnectionStrings _connectionStrings;
		private readonly IDbContext _dbContext;

		public UserRepository(IOptions<ConnectionStrings> connectionStringsOptionsAccessor, IDbContext dbContext)
		{
			_connectionStrings = connectionStringsOptionsAccessor.Value;
			_dbContext = dbContext;
		}

		public List<User> GetAllUsers()
		{
			return _dbContext.Users.ToList();
		}

		public IList<dynamic> GetUserList(string search, string sortCol, bool sortAsc)
		{
			var queryText = $@"
				SELECT
					u.{nameof(User.UserId)}
					,u.{nameof(User.FirstName)}
					,u.{nameof(User.LastName)}
					,u.{nameof(User.Email)}
					,u.{nameof(User.SalesRepID)}
					,u.{nameof(User.IsDeleted)}
					,r.{nameof(Role.RoleName)}
					,r.{nameof(Role.RoleId)}
					,u.{nameof(User.IsHidden)}
				FROM dbo.[User] u
				INNER JOIN dbo.[Role] r ON r.{nameof(Role.RoleId)} = u.{nameof(User.RoleId)}
				WHERE 1 = 1
			";

			if (!string.IsNullOrEmpty(search.Trim()))
			{
				queryText += $@"
					AND (r.{nameof(Role.RoleName)} LIKE :searchTerm
						OR u.{nameof(User.FirstName)} LIKE :searchTerm
						OR u.{nameof(User.LastName)} LIKE :searchTerm
						OR u.{nameof(User.Email)} LIKE :searchTerm
						OR u.{nameof(User.SalesRepID)} LIKE :searchTerm
				)";
			}

			var sortProp = new List<string>(){
				nameof(PurchaseOrder.PurchaseOrderID),
				nameof(PurchaseOrder.PurchaseOrderCustomID),
				nameof(Vendor.VendorCustomID),
				nameof(Vendor.VendorName),
				nameof(PurchaseOrder.ExpectedDate),
				"Items",
				"Amount",
				"PrimaryProductCustomID",
				"PrimaryProductCount"
			}.FirstOrDefault(x => x.ToLower() == sortCol.ToLower());
			queryText += $"ORDER BY {sortCol} {(sortAsc ? "ASC" : "DESC")}";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			if (!string.IsNullOrEmpty(search.Trim()))
			{
				query.SetString("searchTerm", $"%{search}%");
			}

			return query.DynamicList();
		}

		public IList<User> GetUsersWithRoleIn(int[] roleIDs)
		{
			var queryText = $@"
				SELECT *
				FROM dbo.[User] u
				WHERE u.{nameof(User.RoleId)} in (:roleIDs)
			";
			var users = _dbContext.Session.CreateSQLQuery(queryText)
				.SetParameterList("roleIDs", roleIDs.ToList())
				.DynamicList();

			return users.Select(u => new User
			{
				RoleId             = u.RoleId,
				UserId             = u.UserId,
				FirstName          = u.FirstName,
				LastName           = u.LastName,
				Email              = u.Email,
				ForcePasswordReset = u.ForcePasswordReset,
				IsDeleted          = u.IsDeleted,
				SalesRepID         = u.SalesRepID
			}).ToList();
		}

		public List<Role> GetAllRoles()
		{
			return _dbContext.Roles.ToList();
		}

		public Role GetRole(int? roleId)
		{
			return _dbContext.Roles.Where(r => r.RoleId == roleId).FirstOrDefault();
		}

		public Role GetRoleByName(string roleName)
		{
			return _dbContext.Roles.Where(r => r.RoleName == roleName).FirstOrDefault();
		}

		public User GetUser(int userId)
		{
			return _dbContext.Users.Where(u => u.UserId == userId).FirstOrDefault();
		}

		public User GetUser(string email)
		{
			return _dbContext.Users
				.Where(u => email == u.Email && !u.IsDeleted)
				.FirstOrDefault();
		}

		public User GetRepForCustomerShippingInfo(int customerShippingInfoID)
		{
			var queryText = @"
				SELECT u.* FROM [User] u
				JOIN CustomerShippingInfo csi on csi.RepUserID = u.UserId
				WHERE csi.CustomerShippingInfoID = :csiID
			";

			return _dbContext.Session.CreateSQLQuery(queryText)
				.AddEntity(typeof(User))
				.SetInt32("csiID", customerShippingInfoID)
				.List<User>()
				.FirstOrDefault();
		}

		public User SaveUser(User user)
		{
			if (!user.RoleId.HasValue)
				return null;
			var role = _dbContext.Roles.FirstOrDefault( r=> r.RoleId == user.RoleId.Value);
			if (role == null)
				return null;
			user.Role = role;
			_dbContext.BeginTransaction();

			User u;
			if (user.UserId > 0)
			{
				_dbContext.Update(user);
				u = user;
			}
			else
			{
				u = _dbContext.Save(user);
			}

			_dbContext.Commit();

			return u;
		}

		public void DeleteUser(User user)
		{
			//deleted was used instead of disabled, setting IsHidden to hide instead of delete
			var queryText = @"UPDATE [User] SET IsHidden = 1, IsDeleted = 1 WHERE UserId = :userId";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetInt32($"userId", user.UserId);

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();
		}

		public void ResetPassword(int userId, bool forceReset, byte[] password, byte[] salt, DateTime? resetPasswordOn)
		{
			var user = _dbContext.Users.Where(u => u.UserId == userId).FirstOrDefault();
			user.ForcePasswordReset = forceReset;
			user.Password = password;
			user.PasswordSalt = salt;
			user.ResetPasswordOn = resetPasswordOn;

			_dbContext.BeginTransaction();

			_dbContext.Update(user);

			_dbContext.Commit();
		}

		public bool ValidateEmail(int userID, string email)
		{
			return (from user in _dbContext.Users where user.Email == email && user.UserId != userID select user.UserId).Count() == 0;
		}
	}
}
