$app.controller("archivedInvoiceController", [
	"$scope", "$rootScope", "archivedInvoiceService", "$uibModal", "$filter",
	function ($scope, $rootScope, archivedInvoiceService, $uibModal, $filter) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.ArchiveView, SecurityKeys.ArchiveSeeAll, SecurityKeys.ArchiveSeeAllNoTotals, SecurityKeys.ArchiveViewWithTotals]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.ArchiveSeeAllNoTotals, SecurityKeys.ArchiveSeeAll, SecurityKeys.CustomerOrderEdit]);
		$scope.canDoNotFill = $rootScope.checkClaims([SecurityKeys.CustomerDoNotFillFlag]);
		$scope.canFill = $rootScope.checkClaims([SecurityKeys.CustomerOrderFulfillment]);
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

		$scope.canViewTotal = $rootScope.checkClaims([SecurityKeys.ArchiveSeeAll, SecurityKeys.ArchiveViewWithTotals]);


		$scope.table = {
			idField: "customerOrderID",
			hideAdd: true,
			showEdit: false,
			showCheckbox: false,
			hideUpload: true,
			hidePriceImport: true,
			checkboxField: (row) => row.customerOrderShipmentID,
			dataColumns: [
				{ title: "Invoice Date", field: "invoiceDate", hide: false, display: (val) => $filter("localTime")(val, "MM/dd/yyyy") },
				{ title: "Invoice #", field: "invoiceNumber", hide: false, link: (val, row) => `/CustomerOrder/Invoice/${row.customerOrderShipmentID}` },
				{ title: "Order #", field: "customerOrderCustomID", hide: false },
				{ title: "Cust ID", field: "customerCustomID", hide: false},
				{ title: "Cust PO #", field: "poNumber", hide: false },
				{ title: "Invoice Total", field: "invoiceTotal", hide: !$scope.canViewTotal, display: (val) => $filter('currency')(val, "$") }
				
			],
			actionColumns: [
				{ title: "View PO", func: viewPO, hide: false },
				{ title: "View Order", func: viewOrder, hide: false }
			],
			additionalFilters: [

			]
		};

		function viewPO(row, tblScope) {
			Object.assign(document.createElement('a'), {
				target: '_blank',
				href: tblScope.attachmentURI,
			}).click();
		}

		function viewOrder(row, tblScope) {
			debugger;
			Object.assign(document.createElement('a'), {
				href: "CustomerOrder/Details/" + tblScope.customerOrderID,
			}).click();
		}

	}


]);