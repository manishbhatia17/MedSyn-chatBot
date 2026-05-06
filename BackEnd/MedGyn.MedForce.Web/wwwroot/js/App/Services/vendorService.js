$app.service("vendorService", ["$http", function ($http) {
	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction) {
		$http({
			method : method,
			url    : "/api/vendor/" + methodName,
			params : params,
			data   : data,
			headers: { "Content-Type": "application/json" }
		}).then(function (response) {
			if(response.status !== 200) {
				errorFunction(response);
			} else if(successFunction) {
				successFunction(response);
			}
		}, function (response) {
			if(errorFunction !== null)
				errorFunction(response);
		});
	};

	this.fetchVendors = function (searchCriteria, onSuccess, onFailure) {
		makeRequest("POST", "", null, searchCriteria, onSuccess, onFailure);
	};

	this.getVendorDetails = function (vendorID, onSuccess, onFailure) {
		makeRequest("GET", vendorID, null, null, onSuccess, onFailure);
	};

	this.saveVendor = function (data, onSuccess, onFailure) {
		makeRequest("POST", "save", null, data, onSuccess, onFailure);
	};

	this.getProductsForVendor = function (vendorID, onSuccess, onFailure) {
		makeRequest("GET", `${vendorID}/products`, null, null, onSuccess, onFailure);
	};

	return this;
}]);