namespace MedGyn.MedForce.Service.Contracts
{
    public class CustomerChatLogContract
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsExistingCustomer { get; set; }
        public int? CustomerId { get; set; }
    }
}
