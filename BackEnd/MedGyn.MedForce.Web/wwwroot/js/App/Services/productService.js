$app.service("productService", ["$http", function ($http) {
	var makeRequest = function (method, methodName, params, data, options, successFunction, errorFunction) {
		var config = {
			method : method,
			url    : `/api/product/${methodName}`,
			params : params,
			data   : data,
			headers: { "Content-Type": "application/json" },
		}
		if(options) {
			Object.assign(config, options);
		}
		$http(config).then(function (response) {
			if(response.status !== 200) {
				errorFunction(response);
			} else if(successFunction) {
				successFunction(response);
			}
		}, function (response) {
			if(errorFunction !== null)
				errorFunction(response);
		}).catch(function (err) {
			console.log(err);
		});
	};

	this.fetchProducts = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "", params, searchCriteria, null, onSuccess, onFailure);
	};

	this.exportProducts = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "export/excel", params, searchCriteria, {responseType: "blob"}, onSuccess, onFailure);
	};

	this.getProductDetails = function (productID, onSuccess, onFailure) {
		makeRequest("GET", productID, null, null, null, onSuccess, onFailure);
	};

	this.saveProduct = function (data, onSuccess, onFailure) {
		makeRequest("POST", "save", null, data, null, onSuccess, onFailure);
	};
	this.getProductInventoryAdjustments = function (productID, onSuccess, onFailure) {
		makeRequest("GET", `${productID}/adjustments`, null, null, null, onSuccess, onFailure);
	}
	this.saveProductAdjustment = function (data, onSuccess, onFailure) {
		makeRequest("POST", "saveInv", null, data, null, onSuccess, onFailure);
	};

	return this;
}]);
