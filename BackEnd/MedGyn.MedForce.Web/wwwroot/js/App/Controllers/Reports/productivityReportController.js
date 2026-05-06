$app.controller("productivityReportController", [
	"$scope", "$rootScope", "archivedInvoiceService", "$uibModal", "$filter",
	function ($scope, $rootScope, archivedInvoiceService, $uibModal, $filter) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.ProductActivityView]);

		//$scope.isManager = $rootScope.checkClaims([
		//	SecurityKeys.CustomerOrderDomesticManagerApproval,
		//	SecurityKeys.CustomerOrderDomesticDistributorManagerApproval,
		//	SecurityKeys.CustomerOrderInternationalManagerApproval
		//]);
		//$scope.isVP = $rootScope.checkClaims([
		//	SecurityKeys.CustomerOrderDomesticVPApproval,
		//	SecurityKeys.CustomerOrderDomesticDistributorVPApproval,
		//	SecurityKeys.CustomerOrderInternationalVPApproval
		//]);

		$scope.searchValue = "";
		$scope.productId = "";
		$scope.totalQty = 0;
		$scope.totalAmount = 0;
		$scope.searchCriteria = {
			page: 0,
			pageSize: 100,
			search: $scope.searchValue,
			sortAsc: false,
			sortColumn: "submitDate"
		};

		archivedInvoiceService.fetchArchivedInvoices($scope.searchCriteria).then(function (response) {
			const ctx = document.getElementById('packingChart');
			new Chart(ctx, {
				data: {
					labels: ['Packer 1', 'Packer 2', 'Packer 3', 'Packer 4'],
					datasets: [{
						type: 'bar',
						label: 'Orders Packed',
						data: [12, 19, 3, 15, 22, 13],
						borderWidth: 1
					},
					{
						type: 'line',
						label: 'Average Orders Packed Daily',
						data: [7, 3, 1, 5, 9, 3]
					}]
				},
				options: {
					scales: {
						y: {
							beginAtZero: true
						}
					}
				}
			});

			const ctx2 = document.getElementById('shippingChart');

			new Chart(ctx2, {
				data: {
					labels: ['Shipper 1', 'Shipper 2', 'Shipper 3', 'Shipper 4'],
					datasets: [{
						type: 'bar',
						label: 'Orders Shipped',
						data: [12, 9, 35, 13, 22, 13],
						borderWidth: 1
					},
					{
						type: 'line',
						label: 'Average Orders Shipped Daily',
						data: [7, 3, 15, 6, 9, 3]
					}]
				},
				options: {
					scales: {
						y: {
							beginAtZero: true
						}
					}
				}
			});
		});
	
		//$scope.table = {
		//	idField: "customerOrderID",
		//	hideAdd: true,
		//	showEdit: false,
		//	showCheckbox: false,
		//	exportExcelFn: exportList,
		//	allowHtml: true	
		//	dataColumns: [
		//		{ title: "Order Date", field: "submitDate", hide: false, display: (field) => $filter("localTime")(field, "MM/dd/yyyy") },
		//		{ title: "Cust PO #", field: "poNumber" },
		//		{ title: "Order #", field: "customerOrderCustomID" },
		//		{ title: "Cust ID", field: "customerCustomID" },
		//		{ title: "Cust Name", field: "customerName" },
		//		{ title: "Product ID", field: "productCustomID" },
		//		{ title: "Product Name", field: "productName" },
		//		{ title: "Quantity", field: "quantity" },
		//		//{ title: "IsFilling", field: "isFilling", enableHtml: true, render: (data) => { if (data) { return `<i class='greenLight fa fa-circle'></i>${data}`; } else { return "<i class='redLight fa fa-circle'></i>" } } },
		//		{ title: "Being Filled By", field: "FilledBy" }
		//	],

		//	actionColumns: [
		//		{ title: "Fill", func: fill, hide: !$scope.canFill }
		//	],
		//	additionalFilters: [

		//	]
		//};

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

		function SumInvoices(invoices) {

		}
		

		$scope.search = async function () {
			$scope.searchCriteria = {
				page: 0,
				pageSize: 10,
				search: $scope.searchValue,
				sortAsc: true,
				sortColumn: "submitDate"
			};
			
			await archivedInvoiceService.fetchArchivedInvoices($scope.searchCriteria);
		}

		$scope.clear = function () {
			$scope.searchValue = "";
			$scope.productId = "";
			this.search();
		}
	}
])