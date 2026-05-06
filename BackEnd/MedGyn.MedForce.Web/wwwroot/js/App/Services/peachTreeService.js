$app.service("peachTreeService", ["$http", function($http) {
	var makeRequest = function(method, methodName, params, data, successFunction, errorFunction) {
		$http({
			method : method,
			url    : "/api/peachTree/" + methodName,
			params : params,
			data   : data,
			headers: { "Content-Type": "application/json" }
		}).then(function(response) {
			if(response.status !== 200) {
				errorFunction(response);
			} else if(successFunction) {
				successFunction(response);
			}
		}, function(response) {
			if(errorFunction !== null)
				errorFunction(response);
		});
	};

	this.getPreviousInvoiceList = function(params, onSuccess, onFailure) {
	makeRequest("GET", "export/invoice/list", params, null, onSuccess, onFailure);
	};

	this.exportInvoice = function (params, onSuccess, onFailure) {
		makeRequest("GET", "export/invoice", params, null, onSuccess, onFailure, {responseType: "blob"});
	};

	this.getPreviousInvoicesByDate =function (data, onSuccess, onFailure) {
		makeRequest("Post", "export/invoice/ByDate", "", JSON.stringify(data), onSuccess, onFailure, { responseType: "blob" });
	};
	
	this.exportReceipts = function (params, onSuccess, onFailure) {
		makeRequest("GET", "export/receipts", params, null, onSuccess, onFailure, {responseType: "blob"});
	};

	return this;
}]);