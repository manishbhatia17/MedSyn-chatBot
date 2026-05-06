$app.controller("backOrderController", [
	"$scope", "$rootScope", "backOrderService", "$uibModal", "$filter",
	function ($scope, $rootScope, backOrderService, $uibModal, $filter) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.CustomerOrderView, SecurityKeys.BackorderSeeAll, SecurityKeys.BackorderView, SecurityKeys.BackorderViewWithTotals, SecurityKeys.BackorderSeeAllNoTotals]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.BackorderSeeAllNoTotals, SecurityKeys.BackorderSeeAll, SecurityKeys.CustomerOrderEdit]);
		$scope.canDoNotFill = $rootScope.checkClaims([SecurityKeys.CustomerDoNotFillFlag]);
		$scope.canFill = $rootScope.checkClaims([SecurityKeys.CustomerOrderFulfillment]);
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

		$scope.canViewTotal = $rootScope.checkClaims([SecurityKeys.BackorderSeeAll, SecurityKeys.BackorderViewWithTotals]);

		$scope.searchValue = "";
		$scope.productId = "";
		$scope.totalQty = 0;
		$scope.totalAmount = 0;
		$scope.searchCriteria =  {
			page: 0,
			pageSize: 100,
			search: $scope.searchValue,
			sortAsc: false,
			sortColumn: "submitDate",
			productCustomID: $scope.productId
		};

		$scope.table = {
			idField: "customerOrderID",
			hideAdd: true,
			showEdit: false,
			hideUpload: true,
			hidePriceImport: true,
			showCheckbox: false,
			exportExcelFn: exportList,
			allowHtml: true,
			dataColumns: [
				{ title: "Order Date", field: "submitDate", hide: false, display: (field) => $filter("localTime")(field, "MM/dd/yyyy") },
				{ title: "Cust PO #", field: "poNumber" },
				{ title: "Order #", field: "customerOrderCustomID" },
				{ title: "Cust ID", field: "customerCustomID" },
				{ title: "Cust Name", field: "customerName" },
				{ title: "Product ID", field: "productCustomID" },
				{ title: "Product Name", field: "productName"},
				{ title: "Quantity", field: "quantity" },
				//{ title: "Total", field: "total", hide: !$scope.canViewTotal, display: (val) => $filter('currency')(val, "$")},
				//{ title: "IsFilling", field: "isFilling", enableHtml: true, render: (data) => { if (data) { return `<i class='greenLight fa fa-circle'></i>${data}`; } else { return "<i class='redLight fa fa-circle'></i>" } } },
				{ title: "Being Filled By", field: "FilledBy"}
			],

			actionColumns: [
				{ title: "Fill", func: fill,  hide: !$scope.canFill }
			],
			additionalFilters: [

			]
		};

		function fill(customerID, row) {
			location.href = `/CustomerOrder/Fill/${customerID}`;
		}

		function exportList(tblScope) {
			backOrderService.exportProducts(tblScope.searchCriteria, null, (res) => {
				console.log(res);
				var a = document.createElement("a");
				a.href = URL.createObjectURL(new Blob([res.data], { type: "application/excel" }));
				a.target = "_blank";
				a.download = "BackOrders.xlsx";
				document.body.appendChild(a);
				a.click();
				document.body.removeChild(a);
				URL.revokeObjectURL(a.href);
			});
		}


		$scope.search = async function () {
			$scope.searchCriteria = {
				page: 0,
				pageSize: 10,
				search: $scope.searchValue,
				sortAsc: true,
				sortColumn: "submitDate",
				productCustomID: $scope.productId
			};

			await backOrderService.filterBackOrders($scope.searchCriteria);
		}

		$scope.clear = function () {
			$scope.searchValue = "";
			$scope.productId = "";
			this.search();
		}		
	}
])