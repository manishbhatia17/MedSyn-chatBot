using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services.Protocols;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.WebServiceAPI.ShipmentServiceWebReference;

namespace MedGyn.MedForce.WebServiceAPI.ServiceWrappers
{
	public class FedExShipServiceWrapper
	{
		private FedexRateQuoteRequest _request;
		private string _invoiceNumber;

		public FedExShipServiceWrapper(FedexShipmentRequest shipmentRequest)
		{
			_request = shipmentRequest.Request;
			_invoiceNumber = shipmentRequest.InvoiceNumber;
		}

		public FedexShipmentResponse MakeRequest()
		{
			var service = new ShipService();
			var response = new FedexShipmentResponse();

			try
			{
				for(var i = 0; i < _request.Packages.Count(); i++)
				{
					var request = CreateShipmentRequest(i);
					if (!string.IsNullOrEmpty(response.TrackingNumber))
					{
						request.RequestedShipment.MasterTrackingId = new TrackingId()
						{
							TrackingNumber = response.TrackingNumber,
						};
					}

					var reply = service.processShipment(request);
					if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING)
					{
						if (string.IsNullOrEmpty(response.TrackingNumber))
						{
							response.TrackingNumber = reply.CompletedShipmentDetail.CompletedPackageDetails[0].TrackingIds[0].TrackingNumber;
							if (reply.CompletedShipmentDetail.OperationalDetail.DeliveryDateSpecified)
								response.DeliveryDate = reply.CompletedShipmentDetail.OperationalDetail.DeliveryDate;
						}

						var image = reply.CompletedShipmentDetail.CompletedPackageDetails[0].Label.Parts[0].Image;
						response.LabelImages.Add(image);
					}
					else
					{
						response.ErrorMessage = ShowNotifications(reply);
						break;
					}
				}


				return response;
			}
			catch (SoapException e)
			{
				return new FedexShipmentResponse() { ErrorMessage = e.Detail.InnerText };
			}
			catch (Exception e)
			{
				return new FedexShipmentResponse() { ErrorMessage = e.Message };
			}
		}

		private ProcessShipmentRequest CreateShipmentRequest(int idx)
		{
			var request = new ProcessShipmentRequest()
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
			};

			SetShipmentDetails(request);
			SetOrigin(request);
			SetDestination(request);
			SetPayment(request);
			SetLabelDetails(request);
			SetPackageLineItems(request, idx);

			return request;
		}
		private void SetShipmentDetails(ProcessShipmentRequest request)
		{
			request.RequestedShipment = new RequestedShipment
			{
				ShipTimestamp = DateTime.Now,
				ServiceType = _request.ServiceType,
				PackagingType = "YOUR_PACKAGING",
				PackageCount = _request.Packages.Count.ToString(),
			};
		}
		private void SetOrigin(ProcessShipmentRequest request)
		{
			request.RequestedShipment.Shipper = new Party
			{
				Contact = new Contact()
				{
					PersonName = _request.Origin.ContactName,
					CompanyName = _request.Origin.ContactCompany,
					PhoneNumber = _request.Origin.ContactPhone
				},
				Address = GetFedexAddress(_request.Origin)

			};
		}
		private void SetDestination(ProcessShipmentRequest request)
		{
			request.RequestedShipment.Recipient = new Party
			{
				Contact = new Contact()
				{
					PersonName = _request.Recipient.ContactName,
					CompanyName = _request.Recipient.ContactCompany,
					PhoneNumber = _request.Recipient.ContactPhone
				},
				Address = GetFedexAddress(_request.Recipient)
			};
		}
		private void SetPayment(ProcessShipmentRequest request)
		{
			request.RequestedShipment.ShippingChargesPayment = new Payment()
			{
				PaymentType = PaymentType.SENDER,
				Payor = new Payor() { ResponsibleParty = new Party() { AccountNumber = _request.FedExAccountNumber } }
			};
		}
		private void SetLabelDetails(ProcessShipmentRequest request)
		{
			request.RequestedShipment.LabelSpecification = new LabelSpecification
			{
				ImageType = ShippingDocumentImageType.PDF,
				ImageTypeSpecified = true,
				LabelFormatType = LabelFormatType.COMMON2D
			};
		}
		private void SetPackageLineItems(ProcessShipmentRequest request, int idx)
		{
			var packages = new List<RequestedPackageLineItem>();
			var package = _request.Packages[idx];
			packages.Add(new RequestedPackageLineItem()
			{
				SequenceNumber = (idx + 1).ToString(), // package sequence number
				// package weight
				Weight = new Weight
				{
					Units = (WeightUnits) package.WeightUnits,
					Value = package.Weight,
				},
				// package dimensions
				Dimensions = new Dimensions
				{
					Length = ((int)package.Length).ToString(),
					Width = ((int)package.Width).ToString(),
					Height = ((int)package.Height).ToString(),
					Units = (LinearUnits) package.DimensionUnits,
				},
				CustomerReferences = new CustomerReference[1]
				{
					new CustomerReference()
					{
						CustomerReferenceType = CustomerReferenceType.INVOICE_NUMBER,
						Value = _invoiceNumber,
					}
				},
			});
			request.RequestedShipment.RequestedPackageLineItems = packages.ToArray();
		}

		//private string ShowShipmentReply(ProcessShipmentReply reply)
		//{
		//	var ret = string.Empty;
		//	ret += "Shipment Reply details:\n";
		//	ret += "Package details\n";
		//	// Details for each package
		//	foreach (CompletedPackageDetail packageDetail in reply.CompletedShipmentDetail.CompletedPackageDetails)
		//	{
		//		ret += ShowTrackingDetails(packageDetail.TrackingIds);
		//		if (null != packageDetail.PackageRating && null != packageDetail.PackageRating.PackageRateDetails)
		//		{
		//			ret += ShowPackageRateDetails(packageDetail.PackageRating.PackageRateDetails);
		//		}
		//		else
		//		{
		//			ret += "Rate information not returned\n";
		//		}
		//		//ret += ShowBarcodeDetails(packageDetail.OperationalDetail.Barcodes);
		//		//ShowShipmentLabels(packageDetail);
		//	}
		//	//ret += ShowPackageRouteDetails(reply.CompletedShipmentDetail.OperationalDetail);
		//	return ret;
		//}

		//private void ShowShipmentLabels(CompletedPackageDetail packageDetail)
		//{
		//	if (null != packageDetail.Label.Parts[0].Image)
		//	{
		//		// Save outbound shipping label
		//		string LabelPath = "I:\\";
		//		string LabelFileName = LabelPath + packageDetail.TrackingIds[0].TrackingNumber + ".pdf";
		//		SaveLabel(LabelFileName, packageDetail.Label.Parts[0].Image);
		//	}
		//}

		// private string ShowTrackingDetails(TrackingId[] TrackingIds)
		// {
		// 	var ret = string.Empty;
		// 	// Tracking information for each package
		// 	ret += "Tracking details\n";
		// 	if (TrackingIds != null)
		// 	{
		// 		for (int i = 0; i < TrackingIds.Length; i++)
		// 		{
		// 			ret += string.Format("Tracking # {0} Form ID {1}\n", TrackingIds[i].TrackingNumber, TrackingIds[i].FormId);
		// 		}
		// 	}
		// 	return ret;
		// }

		// private string ShowPackageRateDetails(PackageRateDetail[] PackageRateDetails)
		// {
		// 	var ret = string.Empty;
		// 	foreach (PackageRateDetail ratedPackage in PackageRateDetails)
		// 	{
		// 		ret += "\nRate details\n";
		// 		if (ratedPackage.BillingWeight != null)
		// 			ret += string.Format("Billing weight {0} {1}\n", ratedPackage.BillingWeight.Value, ratedPackage.BillingWeight.Units);
		// 		if (ratedPackage.BaseCharge != null)
		// 			ret += string.Format("Base charge {0} {1}\n", ratedPackage.BaseCharge.Amount, ratedPackage.BaseCharge.Currency);
		// 		if (ratedPackage.TotalSurcharges != null)
		// 			ret += string.Format("Total surcharge {0} {1}\n", ratedPackage.TotalSurcharges.Amount, ratedPackage.TotalSurcharges.Currency);
		// 		if (ratedPackage.Surcharges != null)
		// 		{
		// 			// Individual surcharge for each package
		// 			foreach (Surcharge surcharge in ratedPackage.Surcharges)
		// 				ret += string.Format(" {0} surcharge {1} {2}\n", surcharge.SurchargeType, surcharge.Amount.Amount, surcharge.Amount.Currency);
		// 		}
		// 		if (ratedPackage.NetCharge != null)
		// 			ret += string.Format("Net charge {0} {1}\n", ratedPackage.NetCharge.Amount, ratedPackage.NetCharge.Currency);
		// 	}
		// 	return ret;
		// }

		//private string ShowBarcodeDetails(PackageBarcodes barcodes)
		//{
		//	var ret = string.Empty;
		//	// Barcode information for each package
		//	ret += "\nBarcode details\n";
		//	if (barcodes != null)
		//	{
		//		if (barcodes.StringBarcodes != null)
		//		{
		//			for (int i = 0; i < barcodes.StringBarcodes.Length; i++)
		//			{
		//				ret += string.Format("String barcode {0} Type {1}\n", barcodes.StringBarcodes[i].Value, barcodes.StringBarcodes[i].Type);
		//			}
		//		}

		//		if (barcodes.BinaryBarcodes != null)
		//		{
		//			for (int i = 0; i < barcodes.BinaryBarcodes.Length; i++)
		//			{
		//				ret += string.Format("Binary barcode Type {0}\n", barcodes.BinaryBarcodes[i].Type);
		//			}
		//		}
		//	}
		//	return ret;
		//}

		//private string ShowPackageRouteDetails(ShipmentOperationalDetail routingDetail)
		//{
		//	var ret = string.Empty;
		//	ret += string.Format("\nRouting details\n");
		//	ret += string.Format("URSA prefix {0} suffix {1}\n", routingDetail.UrsaPrefixCode, routingDetail.UrsaSuffixCode);
		//	ret += string.Format("Service commitment {0} Airport ID {1}\n", routingDetail.DestinationLocationId, routingDetail.AirportId);

		//	if (routingDetail.DeliveryDaySpecified)
		//	{
		//		ret += $"Delivery day {routingDetail.DeliveryDay}\n";
		//	}
		//	if (routingDetail.DeliveryDateSpecified)
		//	{
		//		ret += $"Delivery date {routingDetail.DeliveryDate.ToShortDateString()}\n";
		//	}
		//	if (routingDetail.TransitTimeSpecified)
		//	{
		//		ret += $"Transit time {routingDetail.TransitTime}\n";
		//	}
		//	return ret;
		//}

		//private void SaveLabel(string labelFileName, byte[] labelBuffer)
		//{
		//	// Save label buffer to file
		//	FileStream LabelFile = new FileStream(labelFileName, FileMode.Create);
		//	LabelFile.Write(labelBuffer, 0, labelBuffer.Length);
		//	LabelFile.Close();
		//}

		private string ShowNotifications(ProcessShipmentReply reply)
		{
			var ret = string.Empty;
			ret += string.Format("Notifications\n");
			for (int i = 0; i < reply.Notifications.Length; i++)
			{
				Notification notification = reply.Notifications[i];
				ret += string.Format("Notification no. {0}\n", i);
				ret += string.Format(" Severity: {0}\n", notification.Severity);
				ret += string.Format(" Code: {0}\n", notification.Code);
				ret += string.Format(" Message: {0}\n", notification.Message);
				ret += string.Format(" Source: {0}\n", notification.Source);
			}
			return ret;
		}

		private ShipmentServiceWebReference.Address GetFedexAddress(Common.SharedModels.Address address)
		{
			return new ShipmentServiceWebReference.Address
			{
				StreetLines = address.StreetLines,
				City = address.City,
				StateOrProvinceCode = address.StateOrProvinceCode,
				PostalCode = address.PostalCode,
				UrbanizationCode = address.UrbanizationCode,
				CountryCode = address.CountryCode,
				CountryName = address.CountryName,
				Residential = address.Residential,
				ResidentialSpecified = address.ResidentialSpecified,
				GeographicCoordinates = address.GeographicCoordinates
			};
		}
	}
}