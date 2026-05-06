$app.controller("priceReconciliationController", [
	"$scope", "$rootScope", "customerOrderService", "$filter",
	function ($scope, $rootScope, customerOrderService, $filter) {
		var me = this;
		var today = new Date();
		var start = new Date().setDate(today.getDate() - Math.abs(32));
		$scope.startDate = new Date(start);
		$scope.endDate = today;

		$rootScope.hello = "hello";
		$scope.table = {
			idField: "CustomerOrderProductFillID",
			exportExcelFn: exportList,
			hideAdd: true,
			hideUpload: true,
			hidePriceImport: true,
			dataColumns: [
				{ title: "Customer Order Product Fill ID", field: "customerOrderProductFillID"},
				{ title: "Customer Order Product ID", field: "customerOrderProductID" },
				{ title: "Quantity Packed", field: "quantityPacked" },
				{ title: "Serial Numbers", field: "serialNumbers" },
				{ title: "Customer Order Shipment Box ID", field: "customerOrderShipmentBoxID" },
				{ title: "Customer Order Shipment ID", field: "customerOrderShipmentID" },
				{ title: "Product ID", field: "productID" },
				{ title: "Product Custom ID", field: "productCustomID" },
				{ title: "Invoice Date", field: "invoiceDate", display: (field) => $filter("localTime")(field, "MM/dd/yyyy") },
				{ title: "Invoice Number", field: "invoiceNumber" },
				{ title: "Price", field: "price", display: (val) => $filter('currency')(val, "$") },
				{ title: "Domestic List Price", field: "priceDomesticList", display: (val) => $filter('currency')(val, "$") },
				{ title: "Domestic Afaxys Price", field: "priceDomesticAfaxys", display: (val) => $filter('currency')(val, "$") },
			{ title: "International Distribution Price", field: "priceInternationalDistribution", display: (val) => $filter('currency')(val, "$") },
				{ title: "Total", field: "invoiceTotal", display: (val) => $filter('currency')(val, "$") },
				{ title: "cust ID", field: "customerCustomID"	},		
			], additionalFilters: [
				{
					type: "date",
					parameter: "startDate",
					value: $scope.startDate,
					onChange: changeFilter,
					text: "Start Date"
				},
				{
					type: "date",
					parameter: "endDate",
					value: $scope.endDate,
					onChange: changeFilter,
					text: " End Date "
				}
			]
		};

		function exportList(tblScope) {
			customerOrderService.exportProductFillList(tblScope.searchCriteria, tblScope.additionalFilters, (res) => {
				console.log(res);
				var a = document.createElement("a");
				a.href = URL.createObjectURL(new Blob([res.data], { type: "application/excel" }));
				a.target = "_blank";
				a.download = "PriceReconciliationReport.xlsx";
				document.body.appendChild(a);
				a.click();
				document.body.removeChild(a);
				URL.revokeObjectURL(a.href);
			});
		}

		function changeFilter() {
			console.log("Hello")
		}
	}
]);