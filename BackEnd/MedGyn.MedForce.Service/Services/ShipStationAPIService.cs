using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Common.Constants;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using System.Net.Http;


namespace MedGyn.MedForce.Service.Services
{
	public class ShipStationAPIService : IShipStationAPIService
	{
		private readonly ShipStationAPISettings _shipStationAPISettings;
		private IRestClient client;
		static HttpClient httpClient;

		static ShipStationAPIService()
        {
			httpClient = new HttpClient();
        }

		public ShipStationAPIService(IOptions<ShipStationAPISettings> shipStationAPISettings)
		{
			_shipStationAPISettings = shipStationAPISettings.Value;
			RestClientOptions options = new RestClientOptions(_shipStationAPISettings.ShipStationApiUrl);
			options.Authenticator = new HttpBasicAuthenticator(_shipStationAPISettings.Username, _shipStationAPISettings.Password);
			
			client = new RestClient(options);
		}

		public List<ShippingRate> GetRatesByCarrier(ShippingRateRequest model)
		{
			var request = new RestRequest(_shipStationAPISettings.ShipStationGetRatesApiUrl);

			var requestObject = model;

			request.AddJsonBody(requestObject);
			
			try
			{
				var response = client.Post(request).Content;
				return JsonConvert.DeserializeObject<List<ShippingRate>>(response);
			}
			catch(Exception ex)
			{
				string msg = ex.Message;
				return new List<ShippingRate>();
			}
		}


		public List<Carrier> GetCarriers()
		{
			var request = new RestRequest(_shipStationAPISettings.ShipStationGetCarriersApiUrl);

			var response = client.GetAsync(request).Result.Content;
			return JsonConvert.DeserializeObject<List<Carrier>>(response);
		}


		public List<CarrierService> GetCarrierServices(string carrierCode)
		{
			var request = new RestRequest(_shipStationAPISettings.ShipStationGetCarrierServicesApiUrl);
			request.AddParameter("carrierCode", carrierCode);
			var response = client.GetAsync(request).Result.Content;
			return JsonConvert.DeserializeObject<List<CarrierService>>(response);
		}

		public List<ShippingPackageType> GetCarrierPackages(string carrierCode)
		{
			var request = new RestRequest(_shipStationAPISettings.ShipStationGetPackagesApiUrl);
			request.AddParameter("carrierCode", carrierCode);
			var response = client.GetAsync(request).Result.Content;
			var returnObj = new List<ShippingPackageType>();
			try
			{
				returnObj = JsonConvert.DeserializeObject<List<ShippingPackageType>>(response);
			}
			catch (Exception)
			{
				//returnObj.Add(new ShippingPackageType { name = "Issue Loading Package Types"});
			}
			return returnObj;
		}

		public ShippingOrderResponse CreateOrder(ShippingOrderRequest model)
		{

			var request = new RestRequest(_shipStationAPISettings.ShipStationCreateOrderApiUrl);
			
			var requestObject = model;

			request.AddJsonBody(requestObject);

			var response = client.Post(request);

			
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				var error = JsonConvert.DeserializeObject<Error>(response.Content);
				return new ShippingOrderResponse()
				{
					errorMessage = error.ExceptionMessage,
					success = false
				};
			}

			var returnResponse = JsonConvert.DeserializeObject<ShippingOrderResponse>(response.Content);
			returnResponse.success = true;
			return returnResponse;
		}

		public CreateLabelResponse CreateOrderLabel(ShippingOrderResponse model)
        {
			try
			{
				dynamic labelRequest = new
				{
					orderId = model.orderId,
					carrierCode = model.carrierCode,
					serviceCode = model.serviceCode,
					packageCode = model.packageCode,
					shipDate = model.shipDate,
					weight = model.weight,
					dimensions = model.dimensions,
					testLabel = false
				};

                string content = JsonConvert.SerializeObject(labelRequest);


                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, _shipStationAPISettings.ShipStationApiUrl + "/" + _shipStationAPISettings.ShipStationCreateOrderLabelApiUrl);
				requestMessage.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(_shipStationAPISettings.Username + ":" + _shipStationAPISettings.Password)));
				requestMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

				HttpResponseMessage responseMessage = httpClient.SendAsync(requestMessage).Result;

				if(responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
				{
                    var error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result);
                    return new CreateLabelResponse()
					{
                        errorMessage = error.ExceptionMessage + ". " + error.Message,
                        success = false
                    };
                }
				else
				{
                    var returnResponse = JsonConvert.DeserializeObject<CreateLabelResponse>(responseMessage.Content.ReadAsStringAsync().Result);
                    returnResponse.success = true;
                    return returnResponse;
                }

				//var request = new RestRequest(_shipStationAPISettings.ShipStationCreateOrderLabelApiUrl);

				//var requestObject = model;

				//request.AddJsonBody(requestObject);

				//var response = client.Post(request);

				//if (response.StatusCode != System.Net.HttpStatusCode.OK)
				//{
				//	var error = JsonConvert.DeserializeObject<Error>(response.Content);
				//	return new CreateLabelResponse()
				//	{
				//		errorMessage = error.ExceptionMessage,
				//		success = false
				//	};
				//}

				//var returnResponse = JsonConvert.DeserializeObject<CreateLabelResponse>(response.Content);
				//returnResponse.success = true;
				//return returnResponse;
			}
			catch(HttpRequestException ex)
			{
				return new CreateLabelResponse()
				{
					errorMessage = ex.Message,
					success = false
				};
			}
			catch(Exception ex)
			{
				return new CreateLabelResponse()
				{
					errorMessage = ex.Message,
					success = false
				};
			}
		}

		[Obsolete("Need to create order to add PO")]
		public CreateLabelResponse CreateLabel(CreateLabelRequest model)
		{
            //if (model.carrierCode != ShippingCarrierCodes.FedExCarrierCode)
            //{
            //    //for testing only-----------------------------
            //    model.testLabel = true;
            //    //---------------------------------------------
            //}
			var request = new RestRequest(_shipStationAPISettings.ShipStationCreateLabelApiUrl);

			var requestObject = model;

			request.AddJsonBody(requestObject);

			var response = client.Post(request).Content;
			var returnResponse = JsonConvert.DeserializeObject<CreateLabelResponse>(response);
			if (returnResponse.labelData == null)
			{
				var error = JsonConvert.DeserializeObject<Error>(response);
				return new CreateLabelResponse()
				{
					errorMessage = error.ExceptionMessage,
					success = false
				};
			}
			returnResponse.success = true;
			return returnResponse;

		}


	}
}
