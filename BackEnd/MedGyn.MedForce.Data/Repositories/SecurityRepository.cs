	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Text;
	using MedGyn.MedForce.Common.Configurations;
	using MedGyn.MedForce.Data.Interfaces;
	using MedGyn.MedForce.Data.Models;
	using Dapper;
	using Microsoft.Extensions.Options;

	namespace MedGyn.MedForce.Data.Repositories
	{
	public class SecurityRepository : ISecurityRepository
	{
		private readonly ConnectionStrings _connectionStrings;
		private readonly IDbContext _dbContext;

		public SecurityRepository(IOptions<ConnectionStrings> connectionStringsOptionsAccessor, IDbContext dbContext)
		{
			_connectionStrings = connectionStringsOptionsAccessor.Value;
			_dbContext = dbContext;
		}

		public bool CanUserAccessPage(int userId, string uri)
		{
			using (SqlConnection conn = new SqlConnection(_connectionStrings.DefaultContext))
			{
				var sql = @"SELECT 
								COUNT(*) AS [ValidKeys]
							FROM
								[Attribute] a
								JOIN[Value] v ON a.AttributeId = v.AttributeId
								JOIN[Role] r ON r.RoleId = v.ValueNumber
								JOIN[RoleSecurityKey] rsk ON r.RoleId = rsk.RoleId
								JOIN[SecurityKeyPage] skp ON rsk.SecurityKeyId = skp.SecurityKeyId
								JOIN[Page] p ON skp.PageId = p.PageId
							WHERE

								a.AttributeName = 'RoleId'
								AND v.EntityId = @UserId
								AND v.TerminateOn > GETDATE()
								AND p.PageUrl = @Uri; ";
				var args = new DynamicParameters();
				args.Add("@Uri", uri);
				args.Add("@UserId", userId);

				var count = conn.QuerySingle<int>(sql, args);
				return count > 0;
			}
		}

		public List<SecurityKey> GetAllSecurityKeys()
		{
			var keys = new List<SecurityKey>();

			using (SqlConnection conn = new SqlConnection(_connectionStrings.DefaultContext))
			{
				var sql = "select * from dbo.[SecurityKey] where IsDeleted = 0 order by SecurityKeyName asc";
				keys = conn.Query<SecurityKey>(sql).ToList();
			}

			return keys;
		}

		public void SaveAllSecurityKeys(List<SecurityKey> securityKeys)
		{
			using (SqlConnection conn = new SqlConnection(_connectionStrings.DefaultContext))
			{
				conn.Open();

				using (var trans = conn.BeginTransaction())
				{
					foreach (var securityKey in securityKeys)
					{
						if (securityKey.SecurityKeyId > 0)
						{
							conn.Execute("update dbo.SecurityKey set SecurityKeyName = @SecurityKeyName, IsDeleted = @IsDeleted where SecurityKeyId = @SecurityKeyId", securityKey, trans);
						}
						else
						{
							conn.Execute("insert into dbo.SecurityKey (SecurityKeyName, IsDeleted) values (@SecurityKeyName, @IsDeleted)", securityKey, trans);
						}
					}

					trans.Commit();
				}
			}
		}

		public Role GetRole(int roleId)
		{
			var role = new Role();

			using (SqlConnection conn = new SqlConnection(_connectionStrings.DefaultContext))
			{
				var sql = "select * from dbo.[Role] where RoleId = @RoleId";
				var args = new DynamicParameters();
				args.Add("@RoleId", roleId);
				role = conn.Query<Role>(sql, args).FirstOrDefault();
			}

			return role;
		}

		public List<SecurityKey> GetAllSecurityKeysForRole(int? roleId)
		{
			var keys = new List<SecurityKey>();
			if (roleId.HasValue) {
				keys = _dbContext.SecurityKeys.Where(sk => sk.Roles.FirstOrDefault(r => r.RoleId == roleId) != null).ToList();
			}
			return keys;
		}

		public List<RoleSecurityKey> GetAllRoleSecurityKeys()
		{
			var roleSecurityKeys = new List<RoleSecurityKey>();

			using (SqlConnection conn = new SqlConnection(_connectionStrings.DefaultContext))
			{
				var sql = "select * from dbo.[RoleSecurityKey]";
				roleSecurityKeys = conn.Query<RoleSecurityKey>(sql).ToList();
			}

			return roleSecurityKeys;
		}

		public void SaveAllRoleSecurityKeys(int roleId, List<RoleSecurityKey> rsks)
		{
			using (SqlConnection conn = new SqlConnection(_connectionStrings.DefaultContext))
			{
				conn.Open();

				using (var trans = conn.BeginTransaction())
				{
					//remove all current mappings
					conn.Execute("delete from dbo.[RoleSecurityKey] where RoleId = @id", new { id = roleId }, trans);

					//add all from user including those that were deleted but still need to be kept
					conn.Execute("insert into dbo.[RoleSecurityKey](RoleId, SecurityKeyId) values (@RoleId, @SecurityKeyId)", rsks, trans);

					trans.Commit();
				}
			}
		}
	}
	}
