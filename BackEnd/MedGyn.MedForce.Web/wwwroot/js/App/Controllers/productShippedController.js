$app.controller("ProductShippedController", [
	"$scope", "$rootScope", "ProductShippedService", "$uibModal", "$filter",
	function ($scope, $rootScope, ProductShippedService, $uibModal, $filter) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.ProductShippedView, SecurityKeys.ProductShippedViewTotals, SecurityKeys.ProductShippedSeeAllNoTotals, SecurityKeys.ProdcutShippedSeeAllWithTotals]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.ProductShippedSeeAllNoTotals, SecurityKeys.ProdcutShippedSeeAllWithTotals, SecurityKeys.CustomerOrderEdit]);
		$scope.canDoNotFill = $rootScope.checkClaims([SecurityKeys.CustomerDoNotFillFlag]);
		$scope.canFill = $rootScope.checkClaims([SecurityKeys.CustomerOrderFulfillment]);
		$scope.canViewProductShippedTotal = $rootScope.checkClaims([SecurityKeys.ProductShippedViewTotals, SecurityKeys.ProductShippedSeeAllNoTotals]);
		$scope.canExport = $rootScope.checkClaims([SecurityKeys.ExportProductList]);

		$scope.isManager = $rootScope.checkClaims([
			SecurityKeys.CustomerOrderDomesticManagerApproval,
			SecurityKeys.CustomerOrderDomesticDistributorManagerApproval,
			SecurityKeys.CustomerOrderInternationalManagerApproval
		]);
		$scope.isVP = $rootScope.checkClaims([
			SecurityKeys.CustomerOrderDomesticVPApproval,
			SecurityKeys.CustomerOrderDomesticDistributorVPApproval,
			SecurityKeys.CustomerOrderInternationalVPApproval
		]);

		var date = new Date();
		var firstDay = new Date(date.getFullYear(), date.getMonth() - 1, 1);
		var lastDay = new Date(date.getFullYear(), date.getMonth(), 0);
		var currentDate = date.toISOString().substring(0, 10);
		document.getElementById('start_date').value = firstDay.toISOString().substring(0, 10);
		document.getElementById('end_date').value = lastDay.toISOString().substring(0, 10);

		$scope.table = {
			idField: "invoiceNumber",
			hideAdd: true,
			showEdit: false,
			showCheckbox: false,
			hideUpload: true,
			hidePriceImport: true,
			exportExcelFn: exportList,
			dataColumns: [
				{ title: "Invoice Date", field: "invoiceDate", hide: false, display: (val) => $filter("localTime")(val, "MM/dd/yyyy") },
				{ title: "Invoice #", field: "invoiceNumber", hide: false },
				{ title: "Order #", field: "customerOrderID", hide: false},
				{ title: "Cust PO #", field: "poNumber", hide: false},
				{ title: "Cust ID", field: "customerID", hide: false },
				{title: "Product ID", field: "productID", hide: false},
				{ title: "Quantity", field: "quantity", hide: false },
				{ title: "Total", field: "lineTotal", hide: !$scope.canViewProductShippedTotal },
				{ title: "Packed By", field: "packedBy", hide: false },
				{title: "Shipped By", field: "shippedBy", hide: false}
			],
			actionColumns: [

			],
			additionalFilters: [

			]
		};

		$scope.filterInvoices = ProductShippedService.filterInvoices;
		$scope.clearFields = ProductShippedService.clearFields;

		function exportList(tblScope) {
			ProductShippedService.exportProducts(tblScope.searchCriteria, null, (res) => {
				var a = document.createElement("a");
				a.href = URL.createObjectURL(new Blob([res.data], { type: "application/excel" }));
				a.target = "_blank";
				a.download = "ProductsShipped.xlsx"

				document.body.appendChild(a);
				a.click();
				document.body.removeChild(a);
				URL.revokeObjectURL(a.href);
			});
		}

	}
]);