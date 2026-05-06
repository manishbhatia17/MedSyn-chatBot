using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Interfaces
{
    public interface ISecurityService
    {
        bool CanUserAccessPage(int userId, string uri);
        List<SecurityKeyContract> GetAllSecurityKeys();
        void SaveAllSecurityKeys(List<SecurityKeyContract> securityKeys);
        List<RoleSecurityKeyContract> GetAllRoleSecurityKeys();
    }
}
