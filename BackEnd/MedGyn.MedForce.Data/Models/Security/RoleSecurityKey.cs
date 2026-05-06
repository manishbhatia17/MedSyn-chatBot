namespace MedGyn.MedForce.Data.Models
{
    public class RoleSecurityKey
    {
        public virtual int RoleSecurityKeyId { get; set; }
        public virtual int RoleId { get; set; }
        public virtual int SecurityKeyId { get; set; }
    }
}
