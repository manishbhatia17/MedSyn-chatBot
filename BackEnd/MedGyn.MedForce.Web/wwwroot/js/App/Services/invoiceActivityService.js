$app.service("invoiceActivityService", ["$http", function ($http) {

	let defaultParams = null;
	let defaultSuccess = null;
	let defaultFailure = null;


	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction, options) {
		var config = {
			method: method,
			url: `/api/Invoices/${methodName}`,
			params: params,
			data: data,
			headers: { "Content-Type": "application/json" },
		}
		if (options) {
			Object.assign(config, options);
		}
		return $http(config).then(function (response) {
				successFunction(response);
		}, function (response) {
			if (errorFunction !== null)
				errorFunction(response);
		});
	};

	this.fetchInvoiceActivity = function (searchCriteria, params, onSuccess, onFailure) {
		defaultParams = params;
		defaultSuccess = onSuccess;
		defaultFailure = onFailure;

		makeRequest("POST", "activity", defaultParams, searchCriteria, defaultSuccess, defaultFailure, null);
	};

	this.filterInvoiceActivity = async function (searchCriteria) {
		makeRequest("POST", "activity", defaultParams, searchCriteria, defaultSuccess, defaultFailure, null);
	};

	this.exportActivity = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "activity/export/excel", params, searchCriteria, onSuccess, onFailure, { responseType: "blob" });
	};

	return this;
}]);