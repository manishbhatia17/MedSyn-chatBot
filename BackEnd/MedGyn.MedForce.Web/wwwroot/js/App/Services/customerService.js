$app.service("customerService", ["$http", function ($http) {
	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction, options) {
		var config = {
			method : method,
			url    : "/api/customer/" + methodName,
			params : params,
			data   : data,
			headers: { "Content-Type": "application/json" }
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
		});
	};

	this.fetchCustomers = function (searchCriteria, onSuccess, onFailure) {
		makeRequest("POST", "", null, searchCriteria, onSuccess, onFailure);
	};

	this.getCustomerDetails = function (customerID, onSuccess, onFailure) {
		makeRequest("GET", customerID, null, null, onSuccess, onFailure);
	};

	this.saveCustomer = function (data, onSuccess, onFailure) {
		makeRequest("POST", "save", null, data, onSuccess, onFailure);
	};

	this.saveCustomerShippingInfo = function (data, onSuccess, onFailure) {
		makeRequest("POST", "shippinginfo/update", null, data, onSuccess, onFailure);
	};

	this.deleteCustomerShippingInfo = function (customerId, customerShippingInfoId, onSuccess, onFailure) {
		makeRequest("DELETE", `${customerId}/shippinginfo/${customerShippingInfoId}`, null, null, onSuccess, onFailure);
	};

	this.exportProducts = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "export/excel", params, searchCriteria, onSuccess, onFailure, {responseType: "blob"});
	};
	
	return this;
}]);