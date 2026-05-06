$app.service("productPriceAdjustmentService", ["$http", function ($http) {
	var makeRequest = function (method, methodName, params, data, options, successFunction, errorFunction) {
		var config = {
			method: method,
			url: `/api/product/${methodName}`,
			params: params,
			data: data,
			headers: { "Content-Type": "application/json" },
		}
		if (options) {
			Object.assign(config, options);
		}
		$http(config).then(function (response) {
			if (response.status !== 200) {
				errorFunction(response);
			} else if (successFunction) {
				successFunction(response);
			}
		}, function (response) {
			if (errorFunction !== null)
				errorFunction(response);
		});
	};

	this.fetchProductPricingInformation = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "price", params	, searchCriteria, null, onSuccess, onFailure);
	};

	this.saveProductPricingInformation = function (data, onSuccess, onFailure) {
		makeRequest("POST", "price/save", null, data, null, onSuccess, onFailure);
	};

	this.exportProducts = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "export/price/excel", params, searchCriteria, { responseType: "blob" }, onSuccess, onFailure);
	};

	this.uploadPrices = function (data, onSuccess, onFailure) {
		makeRequest("POST", "import/price/excel", null, data, null, onSuccess, onFailure);
	};

	return this;
}]);