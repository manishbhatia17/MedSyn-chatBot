$app.controller("customerListController", [
	"$scope", "$rootScope", "customerService",
	function ($scope, $rootScope, customerService) {
		$scope.canView   = $rootScope.checkClaims([SecurityKeys.CustomerView, SecurityKeys.CustomerSeeAll]);
		$scope.canEdit   = $rootScope.checkClaims([SecurityKeys.CustomerEdit]);
		$scope.canExport = $rootScope.checkClaims([SecurityKeys.ExportCustomerList]);
		$scope.canOrder  = $rootScope.checkClaims([SecurityKeys.CustomerOrderEdit]);
		$scope.table     = {
			idField: "customerID",
			exportExcelFn: $scope.canExport ? exportCustomerList : null,
			showEdit: $scope.canEdit || $scope.canView,
			showExcel: $scope.canExport,
			hideAdd: !$scope.canEdit,
			hideUpload: true,
			hidePriceImport: true,
			dataColumns: [
				{ title: "Name", field: "customerName" },
				{ title: "Customer ID", field: "customerCustomID" },
				{ title: "City", field: "city" },
				{ title: "Country", field: "countryCodeID" },
				{ title: "Type", field: "practiceTypeCodeID" },
				{ title: "Status", field: "customerStatusCodeID" },
				{ title: "Payment Type", field: "paymentType" },
			],
			actionColumns: [
				{ title: "Order", func: order, hide: !$scope.canOrder, disabled: disableOrder}
			],
		};

		function order(customerID) {
			location.href = `/customerOrder/details/0?customerID=${customerID}`;
		}

		function disableOrder(row) {
			return row.customerStatusCodeID === "Inactive";
		}

		function exportCustomerList(tblScope) {
			customerService.exportProducts(tblScope.searchCriteria, null, (res) => {
				var a = document.createElement("a");
				a.href = URL.createObjectURL(new Blob([res.data], {type: "application/excel"}));
				a.target = "_blank";
				a.download = "CustomerList.xlsx"

				document.body.appendChild(a);
				a.click();
				document.body.removeChild(a);
				URL.revokeObjectURL(a.href);
			});
		}
	}
]);