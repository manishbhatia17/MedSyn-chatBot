using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Interfaces
{
    public interface ISecurityRepository
    {
        bool CanUserAccessPage(int userId, string uri);
        List<SecurityKey> GetAllSecurityKeys();
        void SaveAllSecurityKeys(List<SecurityKey> securityKeys);
        Role GetRole(int roleId);
        List<SecurityKey> GetAllSecurityKeysForRole(int? roleId);
        List<RoleSecurityKey> GetAllRoleSecurityKeys();
        void SaveAllRoleSecurityKeys(int roleId, List<RoleSecurityKey> rsks);
    }
}
