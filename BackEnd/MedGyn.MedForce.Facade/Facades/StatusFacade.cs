using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Facade.Facades
{
	public class StatusFacade : IStatusFacade
	{
		private readonly AppSettings _appSettings;
		private readonly IDbMigrationService _dbMigrationService;

		public StatusFacade(IOptions<AppSettings> appSettingsOptionsAccessor, IDbMigrationService dbMigrationService)
		{
			_appSettings = appSettingsOptionsAccessor.Value;
			_dbMigrationService = dbMigrationService;
		}

		public StatusViewModel GetStatus()
		{
			var statusViewModel = new StatusViewModel();

			statusViewModel.DatabaseVersion = _appSettings.DatabaseVersion;

			var dbVersion = _dbMigrationService.GetVersion();
			statusViewModel.CurrentVersion = dbVersion;
			statusViewModel.IsDatabaseConnected = !string.IsNullOrEmpty(dbVersion);

			return statusViewModel;
		}

		public StatusViewModel UpdateDatabase(StatusViewModel statusViewModel)
		{
			if (statusViewModel.UpdatePassword != "update-medgyn")
			{
				statusViewModel.IsUpdatedSuccessfully = false;
				return statusViewModel;
			}

			statusViewModel.IsUpdatedSuccessfully = _dbMigrationService.UpdateDatabase(statusViewModel.DatabaseVersion);
			statusViewModel.DatabaseVersion = _appSettings.DatabaseVersion;

			var dbVersion = _dbMigrationService.GetVersion();
			statusViewModel.CurrentVersion = dbVersion;
			statusViewModel.IsDatabaseConnected = !string.IsNullOrEmpty(dbVersion);

			return statusViewModel;
		}
	}
}
