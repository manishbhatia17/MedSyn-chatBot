using System;

namespace MedGyn.MedForce.Data.Models
{
    public class CustomerChatLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsExistingCustomer { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
