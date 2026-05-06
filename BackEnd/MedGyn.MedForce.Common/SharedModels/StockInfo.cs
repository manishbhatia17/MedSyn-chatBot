using System;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class StockInfo
	{
		public StockInfo() { }

		public StockInfo(dynamic stockInfo)
		{
			ProductID         = stockInfo.ProductID;
			AdjustedInventory = stockInfo.AdjustedInventory ?? 0;
			//I dont think i need adjustments anymore we rewrote the query leaving here for now
			AdjustedCOs =  0;
			AdjustedPOs = 0;
			FilledCOs         = (stockInfo.FilledCOs ?? 0);			
			UnfilledCOs       = stockInfo.CommittedCOs != null ? stockInfo.CommittedCOs : 0;
			RecievedPOs       = (stockInfo.RecievedPOs ?? 0);
			PendingPOs        = stockInfo.OpenPOs != null ? stockInfo.OpenPOs : 0;
		}

		public int ProductID { get; set; }
		public int AdjustedInventory { get; set; }
		public int FilledCOs { get; set; }
		public int UnfilledCOs { get; set; }
		public int AdjustedCOs { get; set; }
		public int RecievedPOs { get; set; }
		public int PendingPOs { get; set; }
		public int AdjustedPOs { get; set; }

		public int OnHand => AdjustedInventory - FilledCOs + RecievedPOs;
		public int NetQuantity => OnHand - UnfilledCOs + PendingPOs;
	}
}
