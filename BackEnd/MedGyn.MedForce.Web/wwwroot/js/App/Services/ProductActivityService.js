$app.service("productActivityService", ["$http", function ($http) {

	let defaultParams = null;
	let defaultSuccess = null;
	let defaultFailure = null;


	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction, options) {
		var config = {
			method: method,
			url: `/api/onHandActivity/${methodName}`,
			params: params,
			data: data,
			headers: { "Content-Type": "application/json" },
		}
		if (options) {
			Object.assign(config, options);
		}
		return $http(config).then(function (response) {
			if (response.status !== 200) {
				$('#beginning_qty').html("");
				$('#ending_qty').html("");
				errorFunction(response);
			} else if (successFunction) {
				if (response.data.totalQuantity !== null && response.data.totalQuantity != 0) {
					$('#beginning_qty').html("Beginning Quantity: " + new Intl.NumberFormat().format(response.data.beginningTotal));
					$('#beginning_qty').show();
					$('#ending_qty').html("Ending Quantity: " + new Intl.NumberFormat().format(response.data.endingTotal));;
					$('#ending_qty').show();
				}
				successFunction(response);
			}
		}, function (response) {
			if (errorFunction !== null)
				errorFunction(response);
		});
	};

	this.fetchProductActivity = function (searchCriteria, params, onSuccess, onFailure) {
		defaultParams = params;
		defaultSuccess = onSuccess;
		defaultFailure = onFailure;

		makeRequest("POST", "", defaultParams, searchCriteria, defaultSuccess, defaultFailure, null);
	};

	this.filterProductActivity = async function (searchCriteria) {
		makeRequest("POST", "", defaultParams, searchCriteria, defaultSuccess, defaultFailure, null);
	};

	this.exportProducts = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "export/excel", params, searchCriteria, onSuccess, onFailure, { responseType: "blob" });
	};

	return this;
}]);