using System;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerOrderShipmentViewModel
	{
		public CustomerOrderShipmentViewModel() { }
		public CustomerOrderShipmentViewModel(CustomerOrderShipmentContract cos)
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
			MasterTrackingNumber	= cos.MasterTrackingNumber;
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
		public string MasterTrackingNumber { get; set; }

		public CustomerOrderShipmentContract ToContract()
		{
			return new CustomerOrderShipmentContract()
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
				CreatedBy               = CreatedBy,
				CreatedOn               = CreatedOn,
				MasterTrackingNumber    = MasterTrackingNumber,
			};
		}
	}
}