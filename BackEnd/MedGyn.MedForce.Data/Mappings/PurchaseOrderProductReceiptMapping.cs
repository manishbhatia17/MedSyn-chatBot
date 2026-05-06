using FluentNHibernate.Mapping;
using MedGyn.MedForce.Data.Models;

namespace MedGyn.MedForce.Data.Mappings
{
	public class PurchaseOrderProductReceiptMapping : ClassMap<PurchaseOrderProductReceipt>
	{
		public PurchaseOrderProductReceiptMapping()
		{
			Table("[PurchaseOrderProductReceipt]");
			Id(x => x.PurchaseOrderProductReceiptID)
				.Column("PurchaseOrderProductReceiptID")
				.GeneratedBy.Identity();

				Map(x => x.PurchaseOrderProductID);
				Map(x => x.QuantityReceived);
				Map(x => x.SerialNumbers);
				Map(x => x.ReceiptDate);
		}
	}
}
