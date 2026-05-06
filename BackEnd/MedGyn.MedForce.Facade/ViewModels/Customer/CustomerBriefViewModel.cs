using System.Collections.Generic;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class CustomerBriefViewModel
	{
		public CustomerBriefViewModel() { }

		public CustomerBriefViewModel(
			CustomerContract customer,
			IDictionary<int, CodeContract> countryCodes,
			IDictionary<int, CodeContract> practiceTypeCodes,
			IDictionary<int, CodeContract> customerStatusCodes,
			IDictionary<int, CodeContract> paymentTypeCodes
		)
		{
			CustomerID           = customer.CustomerID;
			CustomerName         = customer.CustomerName;
			CustomerCustomID     = customer.CustomerCustomID;
			City                 = customer.City;
			CountryCodeID        = countryCodes[customer.CountryCodeID]?.CodeDescription;
			CustomerStatusCodeID = customerStatusCodes[customer.CustomerStatusCodeID].CodeDescription;

			if (customer.PracticeTypeCodeID.HasValue)
			{
				PracticeTypeCodeID = customer.PracticeTypeCodeID == -1 ?
					"Other" :
					practiceTypeCodes[customer.PracticeTypeCodeID.Value].CodeDescription;
			}

			if (customer.PaymentTypeCodeID.HasValue)
			{
                PaymentType = paymentTypeCodes[customer.PaymentTypeCodeID.Value].CodeDescription;
            }
			else
			{
				PaymentType = "N/A";
			}
			
		}
		public int CustomerID { get; set; }
		public string CustomerCustomID { get; set; }
		public string CustomerName { get; set; }
		public string City { get; set; }
		public string CountryCodeID { get; set; }
		public string PracticeTypeCodeID { get; set; }
		public string CustomerStatusCodeID { get; set; }
		public string PaymentType { get; set; }
	}
}
