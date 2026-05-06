using System.Web.Http;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.WebServiceAPI.ServiceWrappers;

namespace MedGyn.MedForce.WebServiceAPI.Controllers
{
	[RoutePrefix("api/ups")]
	public class UPSController : ApiController
	{
		[HttpPost, Route("rate")]
		public string GetRateQuote(UPSRateQuoteRequest rateQuoteRequest)
		{
			var service = new UPSRateServiceWrapper(rateQuoteRequest);
			return service.MakeRequest();
		}

		[HttpPost, Route("ship")]
		public UPSShipmentResponse CreateShipment(UPSShipmentRequest shipRequest)
		{
			var rateService = new UPSRateServiceWrapper(shipRequest.Request);
			var response = rateService.MakeRequest();
			if (!decimal.TryParse(response, out _))
				return new UPSShipmentResponse() { ErrorMessage = response };

			var service = new UPSShipServiceWrapper(shipRequest);
			return service.MakeRequest();
		}
	}
}
