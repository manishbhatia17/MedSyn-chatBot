using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Models;
using MedGyn.MedForce.Service.Contracts;
using MedGyn.MedForce.Service.Interfaces;

namespace MedGyn.MedForce.Service.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly ISecurityRepository _securityRepository;

        public SecurityService(ISecurityRepository securityRepository)
        {
            _securityRepository = securityRepository;
        }

        public bool CanUserAccessPage(int userId, string uri)
        {
            return _securityRepository.CanUserAccessPage(userId, uri);
        }

        public List<SecurityKeyContract> GetAllSecurityKeys()
        {
            var keys = new List<SecurityKeyContract>();

            var dbKeys = _securityRepository.GetAllSecurityKeys();
            foreach (var dbKey in dbKeys)
            {
                keys.Add(new SecurityKeyContract(dbKey));
            }

            return keys;
        }

        public void SaveAllSecurityKeys(List<SecurityKeyContract> securityKeys)
        {
            var keys = new List<SecurityKey>();

            foreach (var securityKey in securityKeys)
            {
                keys.Add(securityKey.GetSecurityKey());
            }

            _securityRepository.SaveAllSecurityKeys(keys);
        }

        public List<RoleSecurityKeyContract> GetAllRoleSecurityKeys()
        {
            var rsks = new List<RoleSecurityKeyContract>();

            var dbRsks = _securityRepository.GetAllRoleSecurityKeys();
            foreach (var dbRsk in dbRsks)
            {
                rsks.Add(new RoleSecurityKeyContract(dbRsk));
            }

            return rsks;
        }
    }
}
