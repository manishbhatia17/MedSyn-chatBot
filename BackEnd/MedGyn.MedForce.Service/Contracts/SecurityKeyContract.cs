using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
    public class SecurityKeyContract
    {
        public SecurityKeyContract()
        {

        }

        public SecurityKeyContract(SecurityKey key)
        {
            SecurityKeyId = key.SecurityKeyId;
            SecurityKeyName = key.SecurityKeyName;
            IsDeleted = key.IsDeleted;
        }

        public int SecurityKeyId { get; set; }
        public string SecurityKeyName { get; set; }
        public bool IsDeleted { get; set; }

        public SecurityKey GetSecurityKey()
        {
            return new SecurityKey()
            {
                SecurityKeyId = this.SecurityKeyId,
                SecurityKeyName = this.SecurityKeyName,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
