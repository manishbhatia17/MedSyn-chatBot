using System;
using System.Collections.Generic;
using System.Text;
using MedGyn.MedForce.Common.SharedModels;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IShipStationAPIService
	{
		List<ShippingRate> GetRatesByCarrier(ShippingRateRequest model);
		List<Carrier> GetCarriers();
		List<CarrierService> GetCarrierServices(string carrierCode);
		CreateLabelResponse CreateLabel(CreateLabelRequest model);
		ShippingOrderResponse CreateOrder(ShippingOrderRequest model);
		CreateLabelResponse CreateOrderLabel(ShippingOrderResponse model);
		List<ShippingPackageType> GetCarrierPackages(string carrierCode);
	}
}
