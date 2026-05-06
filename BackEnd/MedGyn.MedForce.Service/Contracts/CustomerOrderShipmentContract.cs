using System;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CustomerOrderShipmentContract
	{
		public CustomerOrderShipmentContract() { }
		public CustomerOrderShipmentContract(CustomerOrderShipment cos)
		{
			CustomerOrderShipmentID = cos.CustomerOrderShipmentID;
			CustomerOrderID         = cos.CustomerOrderID;
			ShipCompanyType         = cos.ShipCompanyType;
			ShipMethodCodeID        = cos.ShipMethodCodeID;
			ShipAccountNumber       = cos.ShipAccountNumber;
			ShipmentComplete        = cos.ShipmentComplete;
			FillOption              = cos.FillOption;
			NumberOfSameBoxes       = cos.NumberOfSameBoxes;
			NumberOfPackingSlips    = cos.NumberOfPackingSlips;
			ShippingCharge          = cos.ShippingCharge;
			CreatedBy               = cos.CreatedBy;
			CreatedOn               = cos.CreatedOn;
			InvoiceNumber           = cos.InvoiceNumber;
			InvoiceDate             = cos.InvoiceDate;
			InvoiceTotal            = cos.InvoiceTotal;
			MasterTrackingNumber    = cos.MasterTrackingNumber;
			InvoiceSent             = cos.InvoiceSent;
			PeachTreeExportBatchID  = cos.PeachTreeExportBatchID;
			DeliveryDate            = cos.DeliveryDate;
		}

		public int CustomerOrderShipmentID { get; set; }
		public int CustomerOrderID { get; set; }
		public int? ShipCompanyType { get; set; }
		public int? ShipMethodCodeID { get; set; }
		public string ShipAccountNumber { get; set; }
		public bool ShipmentComplete { get; set; }
		public int? FillOption { get; set; }
		public int? NumberOfSameBoxes { get; set; }
		public int? NumberOfPackingSlips { get; set; }
		public decimal? ShippingCharge { get; set; }
		public int CreatedBy { get; set; }
		public DateTime CreatedOn { get; set; }
		public string InvoiceNumber { get; set; }
		public DateTime? InvoiceDate { get; set; }
		public decimal? InvoiceTotal { get; set; }
		public string MasterTrackingNumber { get; set; }
		public bool InvoiceSent { get; set; }
		public int? PeachTreeExportBatchID { get; set; }
		public DateTime? DeliveryDate { get; set; }


		public CustomerOrderShipment ToModel()
		{
			return new CustomerOrderShipment()
			{
				CustomerOrderShipmentID = CustomerOrderShipmentID,
				CustomerOrderID         = CustomerOrderID,
				ShipCompanyType         = ShipCompanyType,
				ShipMethodCodeID        = ShipMethodCodeID,
				ShipAccountNumber       = ShipAccountNumber,
				ShipmentComplete        = ShipmentComplete,
				FillOption              = FillOption,
				NumberOfSameBoxes       = NumberOfSameBoxes,
				NumberOfPackingSlips    = NumberOfPackingSlips,
				ShippingCharge          = ShippingCharge,
				InvoiceSent             = InvoiceSent,
				CreatedBy               = CreatedBy,
				CreatedOn               = CreatedOn,
				MasterTrackingNumber    = MasterTrackingNumber,
			};
		}
	}
}