using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Data.Interfaces
{
    public interface IDbMigrationRepository
    {
        string GetVersion();
        bool UpdateDatabase(string newVersion);
    }
}
