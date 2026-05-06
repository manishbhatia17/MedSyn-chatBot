using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.WebServiceAPI.RateServiceWebReference;

namespace MedGyn.MedForce.WebServiceAPI.ServiceWrappers
{
	class FedExRateServiceWrapper
	{
		private FedexRateQuoteRequest _request;

		public FedExRateServiceWrapper(FedexRateQuoteRequest rateQuoteRequest)
		{
			_request = rateQuoteRequest;
		}

		public string MakeRequest()
		{
			var request = CreateRateRequest();
			SetShipmentDetails(request);
			SetOrigin(request);
			SetDestination(request);
			SetPackageLineItems(request);

			var service = new RateService();

			try
			{
				var reply = service.getRates(request);
				var response = string.Empty;
				if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING)
				{
					response += reply.RateReplyDetails[0].RatedShipmentDetails[0].ShipmentRateDetail.TotalNetCharge.Amount.ToString();
				}
				else
				{
					response += ShowNotifications(reply);
				}

				return response;
			}
			catch (SoapException e)
			{
				return e.Detail.InnerText;
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		private RateRequest CreateRateRequest()
		{
			var request = new RateRequest()
			{
				WebAuthenticationDetail = new WebAuthenticationDetail()
				{
					UserCredential = new WebAuthenticationCredential()
					{
						Key = _request.FedExKey,
						Password = _request.FedExPass
					}
				},

				ClientDetail = new ClientDetail()
				{
					AccountNumber = _request.FedExAccountNumber,
					MeterNumber = _request.FedExMeterNumber
				},

				TransactionDetail = new TransactionDetail() { CustomerTransactionId = "WHATEVER VALUE WE WANT" },
				Version = new VersionId(),
				ReturnTransitAndCommit = true,
				ReturnTransitAndCommitSpecified = true,
			};

			return request;
		}

		private void SetShipmentDetails(RateRequest request)
		{
			request.RequestedShipment = new RequestedShipment
			{
				ShipTimestamp = DateTime.Now,
				ShipTimestampSpecified = true,
				//DropoffType = DropoffType.REGULAR_PICKUP,
				ServiceType = _request.ServiceType,
				// request.RequestedShipment.ServiceTypeSpecified = true;
				PackagingType = "YOUR_PACKAGING",
				//PackagingTypeSpecified = true,
				PackageCount = _request.Packages.Count.ToString(),
			};
		}

		private void SetOrigin(RateRequest request)
		{
			request.RequestedShipment.Shipper = new Party
			{
				Address = GetFedexAddress(_request.Origin)
			};
		}

		private void SetDestination(RateRequest request)
		{
			request.RequestedShipment.Recipient = new Party
			{
				Address = GetFedexAddress(_request.Recipient)
			};
		}

		private void SetPackageLineItems(RateRequest request)
		{
			//_request
			var packages = new List<RequestedPackageLineItem>();
			for(var i = 0; i < _request.Packages.Count; i++)
			{
				var package = _request.Packages[i];
				packages.Add(new RequestedPackageLineItem()
				{
					SequenceNumber = (i + 1).ToString(), // package sequence number
					GroupPackageCount = "1",
					// package weight
					Weight = new Weight
					{
						Units = (WeightUnits) package.WeightUnits,
						UnitsSpecified = true,
						Value = package.Weight,
						ValueSpecified = true
					},
					// package dimensions
					Dimensions = new Dimensions
					{
						Length = ((int)package.Length).ToString(),
						Width = ((int)package.Width).ToString(),
						Height = ((int)package.Height).ToString(),
						Units = (LinearUnits) package.DimensionUnits,
						UnitsSpecified = true
					},
				});
			}
			request.RequestedShipment.RequestedPackageLineItems = packages.ToArray();
		}

		// private string ShowRateReply(RateReply reply)
		// {
		// 	var ret = string.Empty;
		// 	ret += "RateReply details:\n";
		// 	foreach (RateReplyDetail rateReplyDetail in reply.RateReplyDetails)
		// 	{
		// 		// if (rateReplyDetail.ServiceTypeSpecified)
		// 		ret += $"Service Type: {rateReplyDetail.ServiceType}";
		// 		// if (rateReplyDetail.PackagingTypeSpecified)
		// 		ret += $"Packaging Type: {rateReplyDetail.PackagingType}\n";
		// 		ret += "\n";
		// 		foreach (RatedShipmentDetail shipmentDetail in rateReplyDetail.RatedShipmentDetails)
		// 		{
		// 			ret += ShowShipmentRateDetails(shipmentDetail);
		// 			ret += "\n";
		// 		}
		// 		ret += ShowDeliveryDetails(rateReplyDetail);
		// 		ret += "**********************************************************\n";
		// 	}

		// 	return ret;
		// }

		// private string ShowShipmentRateDetails(RatedShipmentDetail shipmentDetail)
		// {
		// 	var ret = string.Empty;
		// 	if (shipmentDetail == null) return ret;
		// 	if (shipmentDetail.ShipmentRateDetail == null) return ret;
		// 	ShipmentRateDetail rateDetail = shipmentDetail.ShipmentRateDetail;
		// 	ret += "--- Shipment Rate Detail ---\n";
		// 	//
		// 	ret += $"RateType: {rateDetail.RateType} \n";
		// 	if (rateDetail.TotalBillingWeight != null) ret += $"Total Billing Weight: {rateDetail.TotalBillingWeight.Value} {shipmentDetail.ShipmentRateDetail.TotalBillingWeight.Units}\n";
		// 	if (rateDetail.TotalBaseCharge != null) ret += $"Total Base Charge: { rateDetail.TotalBaseCharge.Amount} {rateDetail.TotalBaseCharge.Currency}\n";
		// 	if (rateDetail.TotalFreightDiscounts != null) ret += $"Total Freight Discounts: {rateDetail.TotalFreightDiscounts.Amount} {rateDetail.TotalFreightDiscounts.Currency}\n";
		// 	if (rateDetail.TotalSurcharges != null) ret += $"Total Surcharges: {rateDetail.TotalSurcharges.Amount} {rateDetail.TotalSurcharges.Currency}\n";
		// 	if (rateDetail.Surcharges != null)
		// 	{
		// 		// Individual surcharge for each package
		// 		foreach (Surcharge surcharge in rateDetail.Surcharges)
		// 			ret += $" {surcharge.SurchargeType} surcharge {surcharge.Amount.Amount} {surcharge.Amount.Currency}\n";
		// 	}
		// 	if (rateDetail.TotalNetCharge != null) ret += $"Total Net Charge: {rateDetail.TotalNetCharge.Amount} {rateDetail.TotalNetCharge.Currency}\n";
		// 	return ret;
		// }

		// private string ShowDeliveryDetails(RateReplyDetail rateDetail)
		// {
		// 	var ret = string.Empty;
		// 	if (rateDetail.DeliveryTimestampSpecified)
		// 		ret += $"Delivery timestamp: { rateDetail.DeliveryTimestamp}\n";
		// 	if (rateDetail.TransitTimeSpecified)
		// 		ret += $"Transit time: {rateDetail.TransitTime}\n";
		// 	return ret;
		// }

		private string ShowNotifications(RateReply reply)
		{
			var ret = "Notifications\n";
			for (int i = 0; i < reply.Notifications.Length; i++)
			{
				Notification notification = reply.Notifications[i];
				ret += $"Notification no. {i}\n";
				ret += $" Severity: {notification.Severity}\n";
				ret += $" Code: {notification.Code}\n";
				ret += $" Message: {notification.Message}\n";
				ret += $" Source: {notification.Source}\n";
			}
			return ret;
		}

		private RateServiceWebReference.Address GetFedexAddress(Common.SharedModels.Address address)
		{
			return new RateServiceWebReference.Address
			{
				StreetLines           = address.StreetLines,
				City                  = address.City,
				StateOrProvinceCode   = address.StateOrProvinceCode,
				PostalCode            = address.PostalCode,
				UrbanizationCode      = address.UrbanizationCode,
				CountryCode           = address.CountryCode,
				CountryName           = address.CountryName,
				Residential           = address.Residential,
				ResidentialSpecified  = address.ResidentialSpecified,
				GeographicCoordinates = address.GeographicCoordinates
			};
		}
	}
}