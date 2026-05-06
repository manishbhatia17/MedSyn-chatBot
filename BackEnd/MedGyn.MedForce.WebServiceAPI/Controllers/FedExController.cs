using System.Web.Http;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.WebServiceAPI.ServiceWrappers;

namespace MedGyn.MedForce.WebServiceAPI.Controllers
{
	[RoutePrefix("api/fedex")]
	public class FedExController : ApiController
	{
		[HttpPost, Route("rate")]
		public string GetRateQuote(FedexRateQuoteRequest rateQuoteRequest)
		{
			var service = new FedExRateServiceWrapper(rateQuoteRequest);
			return service.MakeRequest();
		}

		[HttpPost, Route("ship")]
		public FedexShipmentResponse CreateShipment(FedexShipmentRequest shipRequest)
		{
			var rateService = new FedExRateServiceWrapper(shipRequest.Request);
			var response = rateService.MakeRequest();
			if (!decimal.TryParse(response, out _))
				return new FedexShipmentResponse() { ErrorMessage = response};

			var service = new FedExShipServiceWrapper(shipRequest);
			return service.MakeRequest();
		}
	}
}
