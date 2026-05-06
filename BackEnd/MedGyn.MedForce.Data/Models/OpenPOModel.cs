using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Data.Models
{
	public class OpenPOModel
	{
		public int PurchaseOrderID { get; set; }
		public DateTime ExpectedDate { get; set; }
		public int POs { get; set; }
		public int RecievedPOs { get; set; }
	}
}
