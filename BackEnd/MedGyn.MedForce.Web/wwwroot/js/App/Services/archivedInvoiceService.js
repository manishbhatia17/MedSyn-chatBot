$app.service("archivedInvoiceService", ["$http", function ($http) {
	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction) {
		$http({
			method: method,
			url: "/api/archivedInvoice/" + methodName,
			params: params,
			data: data,
			headers: { "Content-Type": "application/json" }
		}).then(function (response) {
			if (response.status !== 200) {
				console.log('Error');
				console.log(response);
				errorFunction(response);
			} else if (successFunction) {
				console.log('Success');
				console.log(response);
				successFunction(response);
			}
		}, function (response) {
			if (errorFunction !== null)
				errorFunction(response);
		});
	};

	this.fetchArchivedInvoices = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "", params, searchCriteria, onSuccess, onFailure);
	};



	return this;
}]);