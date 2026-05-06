using System;
using System.Collections.Generic;
using System.Linq;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Repositories
{
	public class CodeRepository : ICodeRepository
	{
		private readonly IDbContext _dbContext;

		public CodeRepository(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<CodeType> GetAllCodeTypes(string search, string sortCol, bool sortAsc)
		{
			var query = _dbContext.CodeTypes;
			if(!search.IsNullOrEmpty())
			{
				query = query.Where(x => x.CodeTypeName.Contains(search));
			}
			return query.OrderBy(sortCol, sortAsc);
		}

		public IEnumerable<Code> GetCodesByType(CodeTypeEnum codeType, string search = null, string sortCol = null, bool sortAsc = true)
		{
			var query = _dbContext.Codes.Where(c => c.CodeTypeID == (int)codeType);
			if(!search.IsNullOrEmpty())
			{
				query = query.Where(x => x.CodeName.Contains(search) || x.CodeDescription.Contains(search));
			}
			if(!sortCol.IsNullOrEmpty())
			{
				query = query.OrderBy(sortCol, sortAsc);
			}

			return query;
		}

		public bool SaveCodeTypes(List<CodeType> codeTypes)
		{
			if (!codeTypes.Any())
			{
				return true;
			}

			var tempTableName = $"TempCodeType{Guid.NewGuid().ToString().Replace("-", "")}";
			var queryText = $@"
				DROP TABLE IF EXISTS #{tempTableName}
				SELECT {nameof(CodeType.CodeTypeID)},{nameof(CodeType.CodeTypeName)} INTO #{tempTableName} FROM CodeType where 1 = 0
				INSERT INTO #{tempTableName} ({nameof(CodeType.CodeTypeID)},{nameof(CodeType.CodeTypeName)}) VALUES
			";

			for (var i = 0; i < codeTypes.Count; i++)
			{
				if (i > 0) { queryText += ","; }
				queryText += $"(:codeTypeID{i},:codeTypeName{i})";
			}

			queryText += $@"
				MERGE CodeType as t
					USING(SELECT * FROM #{tempTableName}) as s
						ON s.{nameof(CodeType.CodeTypeID)} = t.{nameof(CodeType.CodeTypeID)}
					WHEN MATCHED THEN
						UPDATE SET
							{nameof(CodeType.CodeTypeName)} = s.{nameof(CodeType.CodeTypeName)}
					;
			";

			var query = _dbContext.Session.CreateSQLQuery(queryText);
			for (var i = 0; i < codeTypes.Count; i++)
			{
				query.SetInt32($"codeTypeID{i}", codeTypes[i].CodeTypeID);
				query.SetString($"codeTypeName{i}", codeTypes[i].CodeTypeName);
			}

			var results = query.ExecuteUpdate();
			return results > 0;
		}

		public bool SaveCodes(List<Code> codes, CodeTypeEnum codeTypeID)
		{
			if (!codes.Any())
			{
				return true;
			}

			var properties = typeof(Code).GetProperties().Select(p => p.Name);
			var tempTableName = $"TempCode{Guid.NewGuid().ToString().Replace("-", "")}";
			var queryText = $@"
				DROP TABLE IF EXISTS #{tempTableName}
				SELECT * INTO #{tempTableName} FROM Code where 1 = 0
				SET IDENTITY_INSERT #{tempTableName} ON
				INSERT INTO #{tempTableName} ({string.Join(',', properties)}) VALUES
			";

			for (var i = 0; i < codes.Count; i++)
			{
				if (i > 0) { queryText += ","; }

				var props = properties.Select(x => $":{x}{i}");
				queryText += $"({string.Join(',', props)})";
			}

			queryText += $@"
				MERGE Code as t
					USING(SELECT * FROM #{tempTableName}) as s
						ON s.CodeID = t.CodeID
					WHEN MATCHED THEN
						UPDATE SET
							CodeName         = s.CodeName
							,CodeDescription = s.CodeDescription
							,IsDeleted       = s.IsDeleted
					WHEN NOT MATCHED BY TARGET THEN
						INSERT VALUES (
							s.CodeName
							,s.CodeDescription
							,s.CodeTypeID
							,s.IsDeleted
							,s.IsRequired
						)
					;
			";


			var query = _dbContext.Session.CreateSQLQuery(queryText);
			for (var i = 0; i < codes.Count; i++)
			{
				query.SetInt32($"CodeID{i}", codes[i].CodeID);
				query.SetString($"CodeName{i}", codes[i].CodeName);
				query.SetString($"CodeDescription{i}", codes[i].CodeDescription);
				query.SetInt32($"CodeTypeID{i}", (int) codeTypeID);
				query.SetBoolean($"IsDeleted{i}", codes[i].IsDeleted);
				query.SetBoolean($"IsRequired{i}", codes[i].IsRequired);
			}

			var results = query.ExecuteUpdate();
			return results > 0;
		}
	}
}
