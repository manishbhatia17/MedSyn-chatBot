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
	public class RoleRepository : IRoleRepository
	{
		private readonly ConnectionStrings _connectionStrings;
		private readonly IDbContext _dbContext;

		public RoleRepository(IOptions<ConnectionStrings> connectionStringsOptionsAccessor, IDbContext dbContext)
		{
			_connectionStrings = connectionStringsOptionsAccessor.Value;
			_dbContext = dbContext;
		}

		public IEnumerable<Role> GetAllRoles()
		{
			return _dbContext.Roles;
		}

		public IList<dynamic> GetRolesList(string search, string sortCol, bool sortAsc = true)
		{
			var queryText = $@"
			Select r.{nameof(Role.RoleId)}, r.{nameof(Role.RoleName)}, r.{nameof(Role.IsArchived)}, UserCount, KeyCount FROM
				[Role] r
				INNER JOIN
				(SELECT r.[{nameof(Role.RoleId)}], count(u.[{nameof(Role.RoleId)}]) [userCount]
				FROM [Role] r 
				Left JOIN [User] u
				ON r.[{nameof(Role.RoleId)}] = u.[{nameof(User.RoleId)}]
				GROUP BY r.[{nameof(Role.RoleId)}]) t1
				ON r.[{nameof(Role.RoleId)}] = t1.[{nameof(Role.RoleId)}]
				INNER JOIN
				(SELECT r.[{nameof(Role.RoleId)}], count(rsk.[{nameof(Role.RoleId)}]) [keyCount]
				FROM [Role] r 
				Left JOIN [RoleSecurityKey] rsk
				ON r.[{nameof(Role.RoleId)}] = rsk.[{nameof(RoleSecurityKey.RoleId)}]
				GROUP BY r.[roleId]) t2
				ON r.[{nameof(Role.RoleId)}] = t2.[{nameof(Role.RoleId)}]";
			if(!string.IsNullOrEmpty(search.Trim()))
			{
				queryText += $@"
					WHERE r.[{nameof(Role.RoleName)}] LIKE :searchTerm
				";
			}
			if(sortCol.IsNullOrEmpty())
			{
				queryText += $" ORDER BY r.{nameof(Role.RoleName)} asc";
			}
			else
			{
				queryText += $@" ORDER BY {sortCol} {(sortAsc ? "" : "desc")}";
			}

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			if(!string.IsNullOrEmpty(search.Trim()))
			{
				query.SetString("searchTerm", $"%{search}%");
			}

			return query.DynamicList();
		}

		public Role GetRole(int? roleId)
		{
			return _dbContext.Roles.Where(r => r.RoleId == roleId).FirstOrDefault();
		}

		public IList<dynamic> GetSecurityKeysForRole(int roleId)
		{
			var queryText = $@"
			SELECT s.[{nameof(SecurityKey.SecurityKeyId)}], s.[{nameof(SecurityKey.SecurityKeyName)}], s.[{nameof(SecurityKey.SecurityKeyDescription)}],
					CASE WHEN [{nameof(RoleSecurityKey.RoleSecurityKeyId)}] IS NULL THEN 0 ELSE 1 END as IsSelected
				FROM SecurityKey s
				LEFT JOIN 
				(SELECT * FROM RoleSecurityKey rsk
				WHERE rsk.RoleId = :roleId) r
				ON s.[{nameof(SecurityKey.SecurityKeyId)}] = r.[{nameof(RoleSecurityKey.SecurityKeyId)}]
				ORDER BY s.[{nameof(SecurityKey.SecurityKeyName)}] ASC
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetInt32("roleId", roleId);

			return query.DynamicList();
		}

		public bool SaveRoles(List<Role> roles)
		{
			if(!roles.Any())
				return true;

			var properties = typeof(Role).GetProperties().Select(p => p.Name);
			var propz = typeof(Role).GetProperties().ToList();
			var tempTableName = $"TempRole{Guid.NewGuid().ToString().Replace("-", "")}";
			var queryText = $@"
				DROP TABLE IF EXISTS #{tempTableName}
				SELECT * INTO #{tempTableName} FROM [Role] where 1 = 0
				SET IDENTITY_INSERT #{tempTableName} ON
				INSERT INTO #{tempTableName} ({string.Join(',', properties)}) VALUES
			";

			for (var i = 0; i < roles.Count; i++)
			{
				if (i > 0) { queryText += ","; }

				var props = properties.Select(x => $":{x}{i}");
				queryText += $"({string.Join(',', props)})";
			}

			queryText += $@"
				MERGE [Role] as t
					USING(SELECT * FROM #{tempTableName}) as s
						ON s.RoleID = t.RoleID
					WHEN MATCHED THEN
						UPDATE SET
							{nameof(Role.RoleName)} = s.{nameof(Role.RoleName)}
							,{nameof(Role.IsArchived)} = s.{nameof(Role.IsArchived)}
					WHEN NOT MATCHED BY TARGET THEN
						INSERT VALUES (
							s.RoleName
							,0
							,0
						)
					;
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			for (var i = 0; i < roles.Count; i++)
			{
				query.SetInt32($"RoleId{i}", roles[i].RoleId);
				query.SetString($"RoleName{i}", roles[i].RoleName);
				query.SetBoolean($"IsArchived{i}", roles[i].IsArchived);
				query.SetBoolean($"IsDeleted{i}", roles[i].IsDeleted);
			}

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();
			return results > 0;
		}

		public bool SaveRolePermissions(int roleId, List<RoleSecurityKey> permissions)
		{
			var properties = typeof(Role).GetProperties().Select(p => p.Name);
			var propz = typeof(Role).GetProperties().ToList();
			var tempTableName = $"TempRole{Guid.NewGuid().ToString().Replace("-", "")}";
			var queryText = $@"
				DELETE FROM [RoleSecurityKey] where [{nameof(RoleSecurityKey.RoleId)}] = :roleId
			";

			if(permissions.Count > 0){
				queryText += $@"INSERT INTO [RoleSecurityKey] ({nameof(RoleSecurityKey.RoleId)}, {nameof(RoleSecurityKey.SecurityKeyId)}) VALUES ";
				for (var i = 0; i < permissions.Count; i++)
				{
					if (i > 0) { queryText += ","; }
					queryText += $"({roleId}, :securityKey{i})";
				}
			}

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			query.SetInt32($"roleId", roleId);
			for (var i = 0; i < permissions.Count; i++)
			{
				query.SetInt32($"securityKey{i}", permissions[i].SecurityKeyId);
			}

			_dbContext.BeginTransaction();
			var results = query.ExecuteUpdate();
			_dbContext.Commit();
			return results > 0;
		}
	}
}
