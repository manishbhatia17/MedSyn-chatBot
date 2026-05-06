using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MedGyn.MedForce.Service.Services
{
	public class UPSService : IUPSService
	{
		private readonly AppSettings _appSettings;
		private readonly ICustomerOrderRepository _customerOrderRepository;

		public UPSService(IOptions<AppSettings> appSettings, ICustomerOrderRepository customerOrderRepository)
		{
			_appSettings             = appSettings.Value;
			_customerOrderRepository = customerOrderRepository;
		}

		public string GetRateQuote(UPSRateQuoteRequest rateQuoteRequest)
		{
			using(var client = new HttpClient())
			{
				var jsonString = JsonConvert.SerializeObject(rateQuoteRequest);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
				var response = client.PostAsync($"{_appSettings.WebServiceAPIEndpoint}/api/ups/rate", content).Result;
				var responseBody = response.Content.ReadAsStringAsync().Result;
				return JsonConvert.DeserializeObject<string>(responseBody);
			}
		}

		public SaveResults CompleteShipment(int shipmentID, UPSShipmentRequest rateQuoteRequest, decimal invoiceTotal)
		{
			UPSShipmentResponse upsResult;
			using(var client = new HttpClient())
			{
				var jsonString = JsonConvert.SerializeObject(rateQuoteRequest);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
				var response = client.PostAsync($"{_appSettings.WebServiceAPIEndpoint}/api/ups/ship", content).Result;
				var responseBody = response.Content.ReadAsStringAsync().Result;
				upsResult = JsonConvert.DeserializeObject<UPSShipmentResponse>(responseBody);
			}

			if(upsResult == null)
				return new SaveResults("Error creating shipment");
			if(!upsResult.ErrorMessage.IsNullOrEmpty())
				return new SaveResults(upsResult.ErrorMessage);

			// UPS does not return a date, so set it as today to be used with COD
			_customerOrderRepository.CompleteShipment(shipmentID, rateQuoteRequest.InvoiceNumber, upsResult.TrackingNumber, DateTime.UtcNow, invoiceTotal);

			var res = new SaveResults();

			using(var memoryStream = new MemoryStream())
			{
				using(var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
				{
					var box = 1;
					foreach(var image in upsResult.LabelImages)
					{
						var zipEntry = archive.CreateEntry($"{rateQuoteRequest.InvoiceNumber}-{box++}.png"); // does not support pdf
						using(var entryStream = new MemoryStream(image))
						using(var zipEntryStream = zipEntry.Open())
							entryStream.CopyTo(zipEntryStream);
					}
				}

				var bytes = memoryStream.ToArray();
				res.Payload = new { ZipFile = Convert.ToBase64String(bytes), Filename = $"{rateQuoteRequest.InvoiceNumber}_Labels"};
			}

			return res;
		}
	}
}
