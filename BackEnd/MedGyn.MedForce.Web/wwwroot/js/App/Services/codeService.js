$app.service("codeService", ["$http", function ($http) {
	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction) {
		$http({
			method : method,
			url    : "/api/code/" + methodName,
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

	this.fetchCodeTypes = function (searchCriteria, onSuccess, onFailure) {
		makeRequest("POST", "", null, searchCriteria, onSuccess, onFailure);
	};

	this.fetchCodes = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "codes", params, searchCriteria, onSuccess, onFailure);
	};

	this.saveCodeTypes = function (data, onSuccess, onFailure) {
		makeRequest("POST", "save", null, data, onSuccess, onFailure);
	};

	this.saveCodes = function (data, params, onSuccess, onFailure) {
		makeRequest("POST", `codes/save`, params, data, onSuccess, onFailure);
	};

	return this;
}]);