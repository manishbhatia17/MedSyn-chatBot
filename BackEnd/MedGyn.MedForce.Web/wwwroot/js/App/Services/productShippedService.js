$app.service("ProductShippedService", ["$http", function ($http) {
	let defaultCriteria = null;
	let defaultParams = null;
	let defaultSuccess = null;
	let defaultFailure = null;

	var makeRequest = function (method, methodName, params, data, successFunction, errorFunction, options, download = false) {
		var config = {
			method: method,
			url: `/api/ProductShipped/${methodName}`,
			params: params,
			data: data,
			headers: { "Content-Type": "application/json" },
		}
		if (options) {
			Object.assign(config, options);
		}
		$http(config).then(function (response) {
			if (!download) {
				if (response.status !== 200) {
					errorFunction(response);
					$('#total_amount').html("");
					$('#total_quantity').html("");
					invoices = response;
				} else if (successFunction) {
					if (response.data.totalAmount != 0 && response.data.totalAmount !== null) {
						$('#total_amount').html("Total Amount : $" + new Intl.NumberFormat().format(response.data.totalAmount.toFixed(2)));
						$('#total_amount').show();
					}
					if (response.data.totalQuantity !== null && response.data.totalQuantity != 0) {
						$('#total_quantity').html("Total Quantity : " + new Intl.NumberFormat().format(response.data.totalQuantity));
						$('#total_quantity').show();
					}
					successFunction(response);
				}
			} else {
				successFunction(response);
			}

			document.getElementById("spinner-wrapper").style.setProperty("display", "none", "important");
		}, function (response) {
			if (errorFunction !== null)
				errorFunction(response);
		});
	};

	var isEmpty = function () {
		if ($('#start_date').val() == "" || $('#product_id').val() === null ||
			$('#end_date').val() == "" || $('#product_id').val() === null) {
			$('#empty_warning').show();
			$('#total_amount').hide();
			$('#total_quantity').hide();
			$('#wrong_date').hide();
			$('#search_title').html("");
			if (defaultCriteria !== null) {
				defaultCriteria.search = JSON.stringify({
					minDate: '2023-01-01',
					maxDate: '2023-12-31',
					productID: 0,
					productName: ""
				});
				makeRequest("POST", "", defaultParams, defaultCriteria, defaultSuccess, defaultFailure);
            }
			return true;
		} else {
			$('#empty_warning').hide();
			$('#wrong_date').hide();
			if (new Date($('#start_date').val()) < new Date($('#end_date').val())) {
				$('#search_title').html("(Product ID : " + $('#product_id').val() + ", Product Name contians: " + $('#product_name').val() + ", Start date: " + $('#start_date').val() + ", End date: " + $('#end_date').val()+")");
				return false;
			} else {
				$('#search_title').html("");
				$('#wrong_date').show();
				return true;
            }
		}
	}

	this.searchInvoice = function (searchCriteria, params, onSuccess, onFailure) {
		defaultCriteria = searchCriteria;
		defaultParams = params;
		defaultSuccess = onSuccess;
		defaultFailure = onFailure;
		if (!isEmpty()) {
			$('#empty_warning').hide();
			defaultCriteria.search = JSON.stringify({
				minDate: $('#start_date').val(),
				maxDate: $('#end_date').val(),
				productID: $('#product_id').val() === null ? 0 : $('#product_id').val(),
				productName: $('#product_name').val() === null ? "" : $('#product_name').val()
			});

			console.log(params);
			makeRequest("POST", "", params, searchCriteria, onSuccess, onFailure);
		}
	};

	this.filterInvoices = function () {
		defaultCriteria.search = JSON.stringify({
			minDate: $('#start_date').val(),
			maxDate: $('#end_date').val(),
			productID: $('#product_id').val(),
			productName: $('#product_name').val()
		});

		

		if (!isEmpty()) {
			$('#empty_warning').hide();
			document.getElementById("spinner-wrapper").style.setProperty("display", "block", "important");
			makeRequest("POST", "", defaultParams, defaultCriteria, defaultSuccess, defaultFailure);
		}
	};

	this.clearFields = function () {
		var date = new Date();
		var firstDay = new Date(date.getFullYear(), date.getMonth() - 1, 1);
		var lastDay = new Date(date.getFullYear(), date.getMonth(), 0);
		var currentDate = date.toISOString().substring(0, 10);
		document.getElementById('start_date').value = firstDay.toISOString().substring(0, 10);
		document.getElementById('end_date').value = lastDay.toISOString().substring(0, 10);
		$('#product_id').val("");
		$('#product_name').val("");

		this.filterInvoices();
	}

	this.exportProducts = function (searchCriteria, params, onSuccess, onFailure) {
		makeRequest("POST", "export/excel", params, searchCriteria, onSuccess, onFailure, { responseType: "blob" }, true);
	};

	isEmpty();

	return this;
}]);