using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.ViewModels.CustomerOrder
{
	public class PeachtreeInvoiceListViewModel
	{
		public int ExportId { get; set; }
		public string InvoiceDate { get; set; }

		public PeachtreeInvoiceListViewModel(dynamic item)
		{
			ExportId = item.AccountingExportBatchID;
			InvoiceDate = item.ExportTS != null ? Convert.ToDateTime(item.ExportTS).ToShortDateString() : "";
		}
	}
}
