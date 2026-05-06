using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Common.SharedModels;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IFedExService
	{
		string GetRateQuote(FedexRateQuoteRequest rateQuoteRequest);
		SaveResults CompleteShipment(int shipmentID, FedexShipmentRequest shipRequest, decimal invoiceTotal);
	}
}
