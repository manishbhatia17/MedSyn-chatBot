namespace MedGyn.MedForce.Service.Contracts
{
    public class ChatShipmentContract
    {
        public int ShipmentId { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsInvoiced { get; set; }
    }
}
