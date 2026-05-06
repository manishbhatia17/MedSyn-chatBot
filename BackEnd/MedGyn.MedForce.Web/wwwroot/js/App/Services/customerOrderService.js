$app.service("customerOrderService", ["$http", function($http) {
	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction, options) {

		var config = {
			method: method,
			url: "/api/customerOrder/" + methodName,
			params: params,
			data: data,
			headers: { "Content-Type": "application/json" }
		}

		if (options) {
			Object.assign(config, options);
		}

		$http(config).then(function(response) {
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

	this.fetchCustomerOrders = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "", params, searchCriteria, onSuccess, onFailure);
	};

	this.fetchFilledOrders = function (searchCriteria, params, onSuccess, onFailure) {

		if (new Date(params.startDate) > new Date(params.endDate)) {
			alert("End Date must be greater than Start Date");
		}

			makeRequest("POST", "fill", params, searchCriteria, onSuccess, onFailure);
		
	};

	this.uploadOrder = function (data, onSuccess, onFailure) {
		makeRequest("POST", "uploadOrder", null, data, onSuccess, onFailure);
	}

	this.exportProductFillList = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "fill/export/excel", params, searchCriteria, onSuccess, onFailure, { responseType: "blob" });
	};

	this.getCustomerOrderDetails = function(customerOrderID, params, onSuccess, onFailure) {
		makeRequest("GET", customerOrderID, params, null, onSuccess, onFailure);
	};

	this.getCustomerOrderFill = function (customerOrderID, params, onSuccess, onFailure) {

		makeRequest("GET", `${customerOrderID}/fill`, params, null, onSuccess, onFailure);
	};

	this.getCustomerOrderShip = function(customerOrderID, shipmentID, params, onSuccess, onFailure) {
		makeRequest("GET", `${customerOrderID}/ship/${shipmentID}`, params, null, onSuccess, onFailure);
	};

	this.saveCustomerOrder = function (data, submit, onSuccess, onFailure) {
		makeRequest("POST", "save", { submit: submit }, data, onSuccess, onFailure);
	};

	this.doNotFill = function (customerOrderID, data, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/donotfill`, null, data, onSuccess, onFailure);
	};
	

	this.getCustomerOrderHistoryForProduct = function(customerID, productID, onSuccess, onFailure) {
		makeRequest("GET", `productHistory`, { productID: productID, customerID: customerID }, null, onSuccess, onFailure);
	};

	this.approveCustomerOrder = function(customerOrderID, isVPApproval, onSuccess, onFailure) {
		var endpoint = isVPApproval ? "vpapprove" : "mgapprove";
		makeRequest("POST", `${customerOrderID}/${endpoint}`, null, null, onSuccess, onFailure);
	};

	this.rescindCustomerOrder = function(customerOrderID, shipmentId, isFilling, onSuccess, onFailure) {
		if(shipmentId)
			makeRequest("POST", `${shipmentId}/rescindShip`, null, null, onSuccess, onFailure);
		else
			makeRequest("POST", `${customerOrderID}/rescind`, { isFilling: isFilling }, null, onSuccess, onFailure);
	};

	this.deleteCustomerOrder = function (customerOrderID, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/delete`, null, null, onSuccess, onFailure);
	};


	this.approveFinancing = function (customerOrderID, data, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/financeapprove`, null, data, onSuccess, onFailure);
	};

	this.fillComplete = function(customerOrderID, data, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/fillComplete`, null, data, onSuccess, onFailure);
	};

	this.addBox = function(customerOrderID, data, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/addBox`, null, data, onSuccess, onFailure);
	};

	this.addShippingBox = function (customerOrderID, shipmentID, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/ship/${shipmentID}/box`, null, null, onSuccess, onFailure);
	};

	this.removeShippingBox = function (customerOrderID, shipmentID, boxID, onSuccess, onFailure) {
		makeRequest("DELETE", `${customerOrderID}/ship/${shipmentID}/box/${boxID}`, null, null, onSuccess, onFailure);
	};

	this.updateBox = function(customerOrderID, boxID, data, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/updateBox/${boxID}`, null, data, onSuccess, onFailure);
	};

	this.updateBoxDims = function(customerOrderID, boxID, data, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/updateBoxDims/${boxID}`, null, data, onSuccess, onFailure);
	};

	this.getRateQuote = function(customerOrderID, shipmentID, currentShip, params, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/getRateQuote/${shipmentID}`, params, currentShip, onSuccess, onFailure);
	}

	this.completeShipment = function (customerOrderID, shipmentID, currentShip, onSuccess, onFailure) {
		makeRequest("POST", `${customerOrderID}/completeShipment/${shipmentID}`, null, currentShip, onSuccess, onFailure)
	}

	this.sendInvoice = function(customerOrderShipmentIDs, onSuccess, onFailure) {
		makeRequest("POST", `Invoice/send`, null, customerOrderShipmentIDs, onSuccess, onFailure);
	};

	return this;
}]);