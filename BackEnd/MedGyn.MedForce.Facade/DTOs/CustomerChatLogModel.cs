namespace MedGyn.MedForce.Facade.DTOs
{
    public class CustomerChatLogModel : IChatWidgetRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsExistingCustomer { get; set; }
        public int? CustomerId { get; set; }
        public string CompanyId { get; set; }
    }
}