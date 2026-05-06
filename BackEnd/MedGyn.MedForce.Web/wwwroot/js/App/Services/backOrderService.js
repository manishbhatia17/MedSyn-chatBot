$app.service("backOrderService", ["$http", function ($http) {

	let defaultParams = null;
	let defaultSuccess = null;
	let defaultFailure = null;


	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction, options) {
		var config = {
			method: method,
			url: `/api/backOrder/${methodName}`,
			params: params,
			data: data,
			headers: { "Content-Type": "application/json" },
		}
		if (options) {
			Object.assign(config, options);
		}
		return $http(config).then(function (response) {
			if (response.status !== 200) {
				$('#total_amount').html("");
				$('#total_quantity').html("");
				errorFunction(response);
			} else if (successFunction) {
				if (response.data.totalQuantity !== null && response.data.totalQuantity != 0) {
					$('#total_quantity').html("Total Quantity : " + new Intl.NumberFormat().format(response.data.totalQty));
					$('#total_quantity').show();
				}
				successFunction(response);
			}
		}, function (response) {
			if (errorFunction !== null)
				errorFunction(response);
		});
	};

	this.fetchBackOrders = function (searchCriteria, params, onSuccess, onFailure) {
		defaultParams = params;
		defaultSuccess = onSuccess;
		defaultFailure = onFailure;
		makeRequest("POST", "", defaultParams, searchCriteria, defaultSuccess, defaultFailure, null);
	};

	this.filterBackOrders = async function(searchCriteria){
		makeRequest("POST", "", defaultParams, searchCriteria, defaultSuccess, defaultFailure, null);
	};

	this.exportProducts = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "export/excel", params, searchCriteria, onSuccess, onFailure, { responseType: "blob" });
	};

	return this;
}]);