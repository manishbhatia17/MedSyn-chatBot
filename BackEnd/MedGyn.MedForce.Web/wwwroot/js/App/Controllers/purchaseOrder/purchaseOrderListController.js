$app.controller("purchaseOrderListController", [
	"$scope", "$rootScope", "purchaseOrderService", "$filter",
	function ($scope, $rootScope, purchaseOrderService, $filter) {
		$scope.canView    = $rootScope.checkClaims([
			SecurityKeys.PurchaseOrderView,
			SecurityKeys.PurchaseOrderDomesticView,
			SecurityKeys.PurchaseOrderDomesticDistributionView,
			SecurityKeys.PurchaseOrderInternationalView,
		]);
		$scope.canEdit    = $rootScope.checkClaims([
			SecurityKeys.PurchaseOrderEdit,
			SecurityKeys.PurchaseOrderDomesticEdit,
			SecurityKeys.PurchaseOrderDomesticDistributionEdit,
			SecurityKeys.PurchaseOrderInternationalEdit,
		]);
		$scope.canReceive = $rootScope.checkClaims([SecurityKeys.PurchaseOrderReceive]);
		$scope.canDoNotReceive = $rootScope.checkClaims([SecurityKeys.PurchaseOrderDoNotReceiveFlag]);
		$scope.canRescind      = $rootScope.checkClaims([SecurityKeys.PurchaseOrderRescind]);
		$scope.isVP       = $rootScope.checkClaims([
			SecurityKeys.PurchaseOrderDomesticVPApproval,
			SecurityKeys.PurchaseOrderDomesticDistributionVPApproval,
			SecurityKeys.PurchaseOrderInternationalVPApproval
		]);

		$scope.statusFilterValues = [];
		if ($scope.canView || $scope.canEdit || $scope.isVP)
			$scope.statusFilterValues.push({ value: 1, text: "Waiting Submission" });
		if($scope.isVP)
			$scope.statusFilterValues.push({ value: 2, text: "Waiting VP Approval" });
		if($scope.canReceive){
			$scope.statusFilterValues.push({ value: 3, text: "To Be Received" });
			$scope.statusFilterValues.push({ value: 5, text: "Has Been Received" });
		}
		if($scope.canDoNotReceive)
			$scope.statusFilterValues.push({ value: 4, text: "Do Not Receive" });

		if ($scope.canView || $scope.canEdit || $scope.isManager || $scope.isVP)
			$scope.statusFilterValues.push({ value: 6, text: "Show my POs" });

		$scope.table = {
			idField    : "purchaseOrderID",
			showEdit   : $scope.canEdit || $scope.canView || $scope.isVP,
			hideAdd: !($scope.canEdit || $scope.isVP),
			hideUpload: true,
			hidePriceImport: true,
			dataColumns: [
				{ title: "PO #", field: "purchaseOrderCustomID" },
				{ title: "Vendor ID", field: "vendorCustomID" },
				{ title: "Vend Name", field: "vendorName" },
				{ title: "Vend Order #", field: "vendorOrderNumber" },
				{ title: "Expt Date", field: "expectedDate" },
				{ title: "Items", field: "items" },
				{ title: "Amount", field: "amount", display: (val) => $filter('currency')(val, "$"), hide: hideAmount },
				{ title: "Pri Prod ID", field: "primaryProductCustomID" },
				{ title: "Pri Prod #", field: "primaryProductCount" },
				{ title: "Pri Lot #", field: "primaryProductLotNumber", hide: hideDateOptions },
				{ title: "Status", field: "status", hide: hideStatus }
			],
			actionColumns: [
				{ title: "Receive", func: receive, hide: hideReceive},
				{ title: "Rescind", func: rescind, hide: hideRescind, disabled: disableRescind },
				{ title: "Delete", func: deletePO, hide: hideDelete },
			],
			additionalFilters: [
				{
					type     : "dropdown",
					parameter: "status",
					values   : $scope.statusFilterValues
				},
				{
					type     : "dropdown",
					parameter: "timeframe",
					disabled: hideDateOptions,
					values   : [
						{value: 1, text: "Today" },
						{value: 2, text: "Yesterday" },
						{value: 3, text: "Last 7 Days" },
						{value: 4, text: "Last 30 Days" },
						{value: 5, text: "This Month" },
						{value: 6, text: "Last Month" },
						{value: 7, text: "This Year" },
						{value: 8, text: "Last Year" }
					]
				}
			]
		};

		function receive(purchaseOrderID) {
			location.href = `/PurchaseOrder/Receive/${purchaseOrderID}`;
		}

		function rescind(purchaseOrderID) {
			if(!$scope.canRescind)
				return;
			purchaseOrderService.rescindPurchaseOrder(purchaseOrderID, (res) => {
				if(res.data) {
					$scope.table.refresh();
				} else {
					alert("Could not approve purchase order")
				}
			}, (err) => alert("Could not approve purchase order"));
		}

		function hideReceive(tblScope) {
			return !$scope.canReceive || tblScope.additionalFilters.status !== 3;
		}

		function hideRescind(tblScope) {
			if(tblScope.additionalFilters.status === 1 || tblScope.additionalFilters.status === 5) {
				return true;
			}
			return !$scope.canRescind;
		}

		function hideDateOptions(tblScope){
			return tblScope.additionalFilters.status !== 5;
		}

		function disableRescind(row, tableScope) {
			if(tableScope.additionalFilters.status == 3 || tableScope.additionalFilters.status == 4) {
				return row.hasReceipts;
		 	}
			return false;
		}


		function hideDelete(tblScope) {
			if (tblScope.additionalFilters.status === 1) {
				return !$scope.canEdit;
			}
			return true;
		}

		function hideStatus(tblScope) {
			console.log("checking status " + tblScope.additionalFilters.status)
			if (tblScope.additionalFilters.status == 6)
				return false;

			return true;
		}

		function deletePO(purchaseOrderID) {
			if (!$scope.canEdit)
				return;
			purchaseOrderService.deletePurchaseOrder(purchaseOrderID, (res) => {
				if (res.data) {
					$scope.table.refresh();
				} else {
					console.error(res.data.errorMessage);
					alert("Could not delete customer po. " + res.data.errorMessage)
				}
			}, (err) => alert("Could not delete customer po. " + err));
		}

		function hideAmount(tblScope) {
			return tblScope.additionalFilters.status === 3;
		}
	}
]);