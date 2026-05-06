using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Common.SharedModels;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IUPSService
	{
		string GetRateQuote(UPSRateQuoteRequest rateQuoteRequest);
		SaveResults CompleteShipment(int shipmentID, UPSShipmentRequest shipmentRequest, decimal invoiceTotal);
	}
}
