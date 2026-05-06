using System;
using System.Linq;
using System.Web.Services.Protocols;
using MedGyn.MedForce.Common.SharedModels;
using MedGyn.MedForce.WebServiceAPI.UPSRateServiceWebReference;

namespace MedGyn.MedForce.WebServiceAPI.ServiceWrappers
{
	public class UPSRateServiceWrapper
	{
		private UPSRateQuoteRequest _request;

		public UPSRateServiceWrapper(UPSRateQuoteRequest request)
		{
			_request = request;
		}

		public string MakeRequest()
		{
			try
			{
				var service = new RateService();
				var request = CreateRateRequest(service);
				SetShipmentDetails(request);
				SetShipperAndOrigin(request);
				SetDestination(request);
				SetPackages(request);

				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11; //This line will ensure the latest security protocol for consuming the web service call.

				var response = service.ProcessRate(request);
				if(response.Response.ResponseStatus.Code == "1")
				{
					return response.RatedShipment[0].TotalCharges.MonetaryValue;
				}
				else
				{
					var alerts = response.Response.Alert.Select(x => x.Description);
					return string.Join(",", alerts);
				}
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

		private RateRequest CreateRateRequest(RateService service)
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

			return new RateRequest
			{
				Request = new RequestType
				{
					RequestOption = new string[] { "Rate" }
				}
			};
		}

		private void SetShipmentDetails(RateRequest request)
		{
			request.Shipment = new ShipmentType
			{
				Service = new CodeDescriptionType
				{
					Code = _request.ServiceType
				}
			};
		}

		private void SetShipperAndOrigin(RateRequest request)
		{
			request.Shipment.Shipper = new ShipperType
			{
				ShipperNumber = _request.AccountNumber,
				Address = new AddressType
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

		private void SetDestination(RateRequest request)
		{
			request.Shipment.ShipTo = new ShipToType
			{
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

		private void SetPackages(RateRequest request)
		{
			request.Shipment.Package = _request.Packages.Select(package =>
				new PackageType
				{
					PackageWeight = new PackageWeightType
					{
						Weight            = package.Weight.ToString(),
						UnitOfMeasurement = new CodeDescriptionType
						{
							Code = package.WeightUnits == 0 ? "KGS" : "LBS",
						}
					},
					Dimensions = new DimensionsType
					{
						Length            = package.Length.ToString(),
						Width             = package.Width.ToString(),
						Height            = package.Height.ToString(),
						UnitOfMeasurement = new CodeDescriptionType
						{
							Code = package.DimensionUnits == 0 ? "CM" : "IN",
						}
					},
					PackagingType = new CodeDescriptionType
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
	}
}