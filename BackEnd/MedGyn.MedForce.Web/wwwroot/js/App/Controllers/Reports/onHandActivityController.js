$app.controller("onHandActivityController", [
	"$scope", "$rootScope", "productActivityService", "productService", "$filter",
	function ($scope, $rootScope, productActivityService, productService, $filter) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.ProductView]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.ProductEdit]);
		$scope.canExport = $rootScope.checkClaims([SecurityKeys.ExportProductList]);
		$scope.canAdjustInventory = $rootScope.checkClaims([SecurityKeys.InventoryAdjust]);
		$scope.canEditPurchaseOrder = $rootScope.checkClaims([SecurityKeys.PurchaseOrderEdit]);

		var today = new Date();
		var start = new Date().setDate(today.getDate() - Math.abs(32));
		$scope.startDate = new Date(start);
		$scope.endDate = today;

		$scope.products = [{value: 0, text: "Select a product..."}];

		$scope.table = {
			idField: "productID",
			showEdit: false,
			hideAdd: true,
			hideUpload: true,
			hidePriceImport: true,
			hideSearch: true,
			isFilterDisabled: true,
			exportExcelFn: $scope.canExport ? exportctivityList : null,
			dataColumns: [
				{ title: "Activity", field: "adjustmentType" },
				{ title: "Order #", field: "orderNumber" },
				{ title: "Date", field: "activityDate", display: (val) => $filter("localTime")(val, "MM/dd/yyyy") },
				{ title: "Units Received", field: "units" },
				{ title: "Adjusted Reason", field: "reason" },
				{ title: "Authorized User", field: "authorizedPerson" },
			],
			actionColumns: [

			],
			additionalFilters: [
				{
					type: "dropdown",
					parameter: "selectedProduct",
					values: $scope.products
				},
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
				},
				
			]
		};

		$scope.beginnigQty = 0;

		$scope.endingQty = 0;

		productService.fetchProducts({ SortColumn: "ProductCustomID" }, {}, (res) => {

			//sort products before pushing to dropdown so we can sort by productCustomID instead of product id
			const results = res.data.results.sort((a, b) => {
				if (a.productCustomID > b.productCustomID)
					return 1;
				if (a.productCustomID < b.productCustomID)
					return -1;
				return 0;
			});

			for (var i = 0; i < results.length; i++) {
				$scope.products.push({ value: res.data.results[i].productID, text: `${res.data.results[i].productCustomID} - ${res.data.results[i].productName}` });
			}

			//$scope.products.sort((a, b) => {
			//	if (a.value > b.value)
			//		return 1;
			//	if (a.value < b.value)
			//		return -1;
   //             return 0;
			//})
		});
		function exportctivityList(tblScope) {
			//productService.exportProducts(tblScope.searchCriteria, null, (res) => {
			//	var a = document.createElement("a");
			//	a.href = URL.createObjectURL(new Blob([res.data], { type: "application/excel" }));
			//	a.target = "_blank";
			//	a.download = "OnHandActivityList.xlsx"

			//	document.body.appendChild(a);
			//	a.click();
			//	document.body.removeChild(a);
			//	URL.revokeObjectURL(a.href);
			//});
		}

		function changeFilter() {
			console.log("Hello")
		}
	}
]);