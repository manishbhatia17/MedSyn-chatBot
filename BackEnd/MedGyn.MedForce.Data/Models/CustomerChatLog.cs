using System;

namespace MedGyn.MedForce.Data.Models
{
    public class CustomerChatLog
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Email { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string State { get; set; }

        public virtual string Country { get; set; }

        public virtual bool IsExistingCustomer { get; set; }

        public virtual int? CustomerId { get; set; }

        public virtual DateTime CreatedAt { get; set; }

    }
}
