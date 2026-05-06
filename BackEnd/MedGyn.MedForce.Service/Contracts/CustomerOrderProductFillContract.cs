using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Service.Contracts
{
	public class CustomerOrderProductFillContract
	{
		public CustomerOrderProductFillContract() { }
		public CustomerOrderProductFillContract(CustomerOrderProductFill customerOrderProductFill)
		{
			CustomerOrderProductFillID = customerOrderProductFill.CustomerOrderProductFillID;
			CustomerOrderProductID     = customerOrderProductFill.CustomerOrderProductID;
			QuantityPacked             = customerOrderProductFill.QuantityPacked;
			SerialNumbers              = customerOrderProductFill.SerialNumbers;
			CustomerOrderShipmentBoxID = customerOrderProductFill.CustomerOrderShipmentBoxID;
		}

		public int CustomerOrderProductFillID { get; set; }
		public int CustomerOrderProductID { get; set; }
		public int QuantityPacked { get; set; }
		public string SerialNumbers { get; set; }
		public virtual int? CustomerOrderShipmentBoxID { get; set; }


		public CustomerOrderProductFill ToModel()
		{
			return new CustomerOrderProductFill()
			{
				CustomerOrderProductFillID = CustomerOrderProductFillID,
				CustomerOrderProductID     = CustomerOrderProductID,
				QuantityPacked             = QuantityPacked,
				SerialNumbers              = SerialNumbers,
				CustomerOrderShipmentBoxID = CustomerOrderShipmentBoxID,
			};
		}
	}
}
