using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
	public class DbMigrationService : IDbMigrationService
	{
		private readonly IDbMigrationRepository _dbMigrationRepository;

		public DbMigrationService(IDbMigrationRepository dbMigrationRepository)
		{
			_dbMigrationRepository = dbMigrationRepository;
		}

		public string GetVersion()
		{
			return _dbMigrationRepository.GetVersion();
		}

		public bool UpdateDatabase(string newVersion)
		{
			return _dbMigrationRepository.UpdateDatabase(newVersion);
		}
	}
}
