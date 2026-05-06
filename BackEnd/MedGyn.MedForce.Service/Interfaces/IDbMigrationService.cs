using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IDbMigrationService
	{
		string GetVersion();
		bool UpdateDatabase(string newVersion);
	}
}
