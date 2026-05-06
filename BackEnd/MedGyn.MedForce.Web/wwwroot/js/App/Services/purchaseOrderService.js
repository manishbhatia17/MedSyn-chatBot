$app.service("purchaseOrderService", ["$http", function ($http) {
	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction) {
		$http({
			method : method,
			url    : "/api/purchaseOrder/" + methodName,
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

	this.fetchPurchaseOrders = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "", params, searchCriteria, onSuccess, onFailure);
	};

	this.getPurchaseOrderDetails = function (purchaseOrderID, params, onSuccess, onFailure) {
		makeRequest("GET", purchaseOrderID, params, null, onSuccess, onFailure);
	};

	this.sendPurchaseOrderReport = function (purchaseOrderID, onSuccess, onFailure) {
		makeRequest("GET", `SendReport/${purchaseOrderID}`, null, null,  onSuccess, onFailure);
	};

	this.getPurchaseOrderReceive = function (purchaseOrderID, params, onSuccess, onFailure) {
		makeRequest("GET", `${purchaseOrderID}/receive`, params, null, onSuccess, onFailure);
	};

	this.savePurchaseOrder = function (data, submit, onSuccess, onFailure) {
		makeRequest("POST", "save", { submit: submit }, data, onSuccess, onFailure);
	};

	this.receiptComplete = function(purchaseOrderID, data, onSuccess, onFailure) {
		makeRequest("POST", `${purchaseOrderID}/receiptComplete`, null, data, onSuccess, onFailure);
	}

	this.getPurchaseOrderHistoryForProduct = function (productID, onSuccess, onFailure) {
		makeRequest("GET", `productHistory`, { productID: productID }, null, onSuccess, onFailure);
	};

	this.approvePurchaseOrder = function(purchaseOrderID, onSuccess, onFailure) {
		makeRequest("POST", `${purchaseOrderID}/approve`, null, null, onSuccess, onFailure);
	}
	this.rescindPurchaseOrder = function(purchaseOrderID, onSuccess, onFailure) {
		makeRequest("POST", `${purchaseOrderID}/rescind`, null, null, onSuccess, onFailure);
	}

	this.deletePurchaseOrder = function (purchaseOrderID, onSuccess, onFailure) {
		makeRequest("DELETE", purchaseOrderID, null, null, onSuccess, onFailure);
	}


	return this;
}]);
