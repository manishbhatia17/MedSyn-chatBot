using System;
using System.Linq;
using System.Web.Services.Protocols;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.WebServiceAPI.UPSShipServiceWebReference;

namespace MedGyn.MedForce.WebServiceAPI.ServiceWrappers
{
	public class UPSShipServiceWrapper
	{
		private UPSRateQuoteRequest _request;
		private string _invoiceNumber;

		public UPSShipServiceWrapper(UPSShipmentRequest shipRequest)
		{
			_request = shipRequest.Request;
			_invoiceNumber = shipRequest.InvoiceNumber;
		}

		public UPSShipmentResponse MakeRequest()
		{
			try
			{
				var service = new ShipService();

				var request = CreateShipmentRequest(service);
				SetShipmentDetails(request);
				SetPayment(request);
				SetShipperAndOrigin(request);
				SetDestination(request);
				SetPackages(request);
				SetLabelDetails(request);

				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11; //This line will ensure the latest security protocol for consuming the web service call.
				var response = service.ProcessShipment(request);

				return new UPSShipmentResponse() {
					TrackingNumber = response.ShipmentResults.ShipmentIdentificationNumber,
					LabelImages = response.ShipmentResults.PackageResults.Select(x => 
						Convert.FromBase64String(x.ShippingLabel.GraphicImage)
					).ToList()
				};
			}
			catch (SoapException e)
			{
				return new UPSShipmentResponse() { ErrorMessage = e.Detail.InnerText };
			}
			catch (Exception e)
			{
				return new UPSShipmentResponse() { ErrorMessage = e.Message };
			}
		}

		private ShipmentRequest CreateShipmentRequest(ShipService service)
		{
			service.UPSSecurityValue = new UPSSecurity
			{
				ServiceAccessToken = new UPSSecurityServiceAccessToken
				{
					AccessLicenseNumber = _request.AccessLicenseNumber
				},
				UsernameToken = new UPSSecurityUsernameToken
				{
					Username = _request.Username,
					Password = _request.Password
				}
			};

			return new ShipmentRequest
			{
				Request = new RequestType
				{
					RequestOption = new string[] { "validate" }
				}
			};
		}

		private void SetShipmentDetails(ShipmentRequest request)
		{
			request.Shipment = new ShipmentType
			{
				Service = new ServiceType
				{
					Code = _request.ServiceType
				}
			};
		}

		private void SetPayment(ShipmentRequest request)
		{
			request.Shipment.PaymentInformation = new PaymentInfoType
			{
				ShipmentCharge = new ShipmentChargeType[]
				{
					new ShipmentChargeType
					{
						BillShipper = new BillShipperType
						{
							AccountNumber = _request.AccountNumber
						},
						Type = "01"
						//01 = Transportation
						//02 = Duties and Taxes
						//03 = Broker of Choice
					}
				}
			};
		}

		private void SetShipperAndOrigin(ShipmentRequest request)
		{
			request.Shipment.Shipper = new ShipperType
			{
				ShipperNumber = _request.AccountNumber,
				Name          = _request.Origin.ContactCompany,
				Phone         = new ShipPhoneType
				{
					Number = _request.Origin.ContactPhone
				},
				Address = new ShipAddressType
				{
					AddressLine       = _request.Origin.StreetLines,
					City              = _request.Origin.City,
					PostalCode        = _request.Origin.PostalCode,
					StateProvinceCode = _request.Origin.StateOrProvinceCode,
					CountryCode       = _request.Origin.CountryCode
				}
			};

			request.Shipment.ShipFrom = new ShipFromType
			{
				Name = _request.Origin.ContactCompany,
				Phone = new ShipPhoneType
				{
					Number = _request.Origin.ContactPhone
				},
				Address = new ShipAddressType
				{
					AddressLine       = _request.Origin.StreetLines,
					City              = _request.Origin.City,
					PostalCode        = _request.Origin.PostalCode,
					StateProvinceCode = _request.Origin.StateOrProvinceCode,
					CountryCode       = _request.Origin.CountryCode
				}
			};
		}

		private void SetDestination(ShipmentRequest request)
		{
			request.Shipment.ShipTo = new ShipToType
			{
				Name = _request.Recipient.ContactName,
				Phone = new ShipPhoneType
				{
					Number = _request.Recipient.ContactPhone
				},
				Address = new ShipToAddressType
				{
					AddressLine       = _request.Recipient.StreetLines,
					City              = _request.Recipient.City,
					PostalCode        = _request.Recipient.PostalCode,
					StateProvinceCode = _request.Recipient.StateOrProvinceCode,
					CountryCode       = _request.Recipient.CountryCode
				}
			};
		}

		private void SetPackages(ShipmentRequest request)
		{
			request.Shipment.Package = _request.Packages.Select(package =>
				new PackageType
				{
					PackageWeight = new PackageWeightType
					{
						Weight = package.Weight.ToString(),
						UnitOfMeasurement = new ShipUnitOfMeasurementType
						{
							Code = package.WeightUnits == 1 ? "KGS" : "LBS",
						}
					},
					Dimensions = new DimensionsType
					{
						Length            = package.Length.ToString(),
						Width             = package.Width.ToString(),
						Height            = package.Height.ToString(),
						UnitOfMeasurement = new ShipUnitOfMeasurementType
						{
							Code = package.DimensionUnits == 1 ? "CM" : "IN",
						}
					},
					Packaging = new PackagingType
					{
						Code = "02"
						//01 Bag
						//02 Box
						//03 Carton / Piece
						//04 Crate
						//05 Drum
						//06 Pallet / Skid
						//07 Roll
						//08 Tube
					}
				}
			).ToArray();
		}

		private void SetLabelDetails(ShipmentRequest request)
		{
			request.LabelSpecification = new LabelSpecificationType
			{
				LabelStockSize = new LabelStockSizeType
				{
					Height = "6",
					Width = "4"
				},
				LabelImageFormat = new LabelImageFormatType
				{
					Code = "PNG" // UPS does not support PDF
				}
			};
		}
	}
}