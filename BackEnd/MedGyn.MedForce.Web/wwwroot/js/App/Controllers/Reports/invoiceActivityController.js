$app.controller("invoiceActivityController", [
	"$scope", "$rootScope", "invoiceActivityService", "$filter",
	function ($scope, $rootScope, invoiceActivityService, $filter) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.ProductView]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.ProductEdit]);
		$scope.canExport = $rootScope.checkClaims([SecurityKeys.ExportProductList]);
		$scope.canAdjustInventory = $rootScope.checkClaims([SecurityKeys.InventoryAdjust]);
		$scope.canEditPurchaseOrder = $rootScope.checkClaims([SecurityKeys.PurchaseOrderEdit]);

		var today = new Date();
		var start = new Date().setDate(today.getDate() - Math.abs(32));
		$scope.startDate = new Date(start);
		$scope.endDate = today;

		$scope.table = {
			idField: "orderNumber",
			showEdit: false,
			hideAdd: true,
			hideUpload: true,
			hidePriceImport: true,
			hideSearch: true,
			isFilterDisabled: true,
			exportExcelFn: $scope.canExport ? exportactivityList : null,
			dataColumns: [
				{ title: "Invoice Number", field: "invoiceNumber" },
				{ title: "Invoice Date", field: "invoiceDate", display: (val) => $filter("localTime")(val, "MM/dd/yyyy") }, 
				{ title: "Order #", field: "customerOrderCustomID" },
				{ title: "Company", field: "company" }, 
				{ title: "Primary Contact", field: "contact"},
				{ title: "Primary Email", field: "email" },
				{ title: "Primary Phone", field: "phone" },
				{ title: "Practice Type", field: "practice" },
                
			],
			actionColumns: [

			],
			additionalFilters: [
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


		function exportactivityList(tblScope) {
			invoiceActivityService.exportActivity(tblScope.searchCriteria, tblScope.additionalFilters, (res) => {
				var a = document.createElement("a");
				a.href = URL.createObjectURL(new Blob([res.data], { type: "application/excel" }));
				a.target = "_blank";
				a.download = `InvoiceActivity_${new Date().toISOString().slice(0, 10)}.xlsx`;

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