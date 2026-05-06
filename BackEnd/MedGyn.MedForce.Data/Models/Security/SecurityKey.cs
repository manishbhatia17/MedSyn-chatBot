using System.Collections.Generic;

namespace MedGyn.MedForce.Data.Models
{
    public class SecurityKey
    {
        public SecurityKey()
        {
            Roles = new List<Role>();
        }

        public virtual int SecurityKeyId { get; set; }
        public virtual string SecurityKeyName { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual string SecurityKeyDescription { get; set; }

        public virtual IList<Role> Roles { get; set; }
    }
}
