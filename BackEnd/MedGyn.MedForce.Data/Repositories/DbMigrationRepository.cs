using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Dapper;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Data.Repositories
{
	public class DbMigrationRepository : IDbMigrationRepository
	{
		private readonly ConnectionStrings _connectionStrings;
		private readonly AppSettings _appSettings;

		public DbMigrationRepository(IOptions<ConnectionStrings> connectionStringsOptionsAccessor, IOptions<AppSettings> appSettingsOptionsAccessor)
		{
			_connectionStrings = connectionStringsOptionsAccessor.Value;
			_appSettings = appSettingsOptionsAccessor.Value;
		}

		public string GetVersion()
		{
			using (SqlConnection conn = new SqlConnection(_connectionStrings.DefaultContext))
			{
				try
				{
					conn.Open();
				}
				catch (Exception)
				{
					return null;
				}

				var sql = "SELECT * FROM dbo.[_DBVersion];";
				List<DBVersion> dbVersions;
				try
				{
					dbVersions = conn.Query<DBVersion>(sql).ToList();
				}
				catch (Exception)
				{
					return string.Empty;
				}

				dbVersions.Sort((a, b) => a.Version.CompareTo(b.Version));
				return dbVersions.LastOrDefault()?.Version ?? string.Empty;
			}
		}

		public bool UpdateDatabase(string newVersion)
		{
			using (var conn = new SqlConnection(_connectionStrings.DefaultContext))
			{
				try
				{
					conn.Open();
				}
				catch (Exception)
				{
					return false;
				}

				var sql = "SELECT * FROM dbo.[_DBVersion];";
				List<DBVersion> dbVersions;
				try
				{
					dbVersions = conn.Query<DBVersion>(sql).ToList();
				}
				catch (Exception)
				{
					dbVersions = new List<DBVersion>();
				}
				dbVersions.Sort((x, y) => x.Version.CompareTo(y.Version));

				var currentVersion = dbVersions.LastOrDefault()?.Version ?? string.Empty;

				using (var trans = conn.BeginTransaction())
				{
					var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + _appSettings.MigrationFileFolder;

					var fileNames = new List<string>(Directory.GetFiles(currentDirectory, "*.sql"));
					fileNames.Sort();

					foreach (string filename in fileNames)
					{
						var fileInfo = new FileInfo(filename);
						var name = fileInfo.Name.Split("_");
						if (name.Length == 1)
						{
							continue;
						}

						bool parsed = ParseDate(name[0], out var filedate);
						if (!parsed)
						{
							continue;
						}

						parsed = ParseDate(currentVersion, out var currentdate);
						// If the current date is empty (table does not exist) then
						// The min time will be parsed
						if (!parsed && currentdate != DateTime.MinValue)
						{
							break;
						}

						DateTime newDate;
						parsed = ParseDate(newVersion, out newDate);
						if (!parsed)
						{
							break;
						}

						if ((dbVersions.Any(d => d.Version == name[0]) && filedate <= currentdate) || filedate > newDate)
						{
							continue;
						}

						var sqlText = File.ReadAllText(filename);

						var sqlCommand = conn.CreateCommand();
						sqlCommand.Transaction = trans;

						sqlCommand.CommandText = sqlText;
						sqlCommand.CommandType = System.Data.CommandType.Text;
						sqlCommand.ExecuteNonQuery();

						var insertCommand = conn.CreateCommand();
						insertCommand.Transaction = trans;

						insertCommand.CommandText =
							$"INSERT INTO [dbo].[_DBVersion] ([Version]) VALUES ('{name[0]}')";
						insertCommand.CommandType = System.Data.CommandType.Text;
						insertCommand.ExecuteNonQuery();
					}

					var rerunnableFiles = new List<string>(Directory.GetFiles(currentDirectory + "Rerunnable", "*.sql"));
					foreach(var filename in rerunnableFiles) {
						var sqlText = File.ReadAllText(filename);

						var sqlCommand = conn.CreateCommand();
						sqlCommand.Transaction = trans;

						sqlCommand.CommandText = sqlText;
						sqlCommand.CommandType = System.Data.CommandType.Text;
						sqlCommand.CommandTimeout = 120;
						sqlCommand.ExecuteNonQuery();
					}

					trans.Commit();
				}
			}

			return true;
		}

		private static bool ParseDate(string newVersion, out DateTime newDate)
		{
			return DateTime.TryParseExact(newVersion, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out newDate);
		}
	}
}
