$app.controller("navController", [
	"$scope", "$rootScope", "$uibModal", "peachTreeService",
	function ($scope, $rootScope, $uibModal, peachTreeService) {	
		$scope.init = function (claims, userId) {
			$rootScope.claims = claims;
			$rootScope.userId = userId;

			if (!isFunction($rootScope.checkClaims)) {
				$rootScope.checkClaims = function (claimKeys) {
					let hasClaim = false;
					claimKeys.forEach(function (claimKey) {
						try {
							if ($rootScope.claims.indexOf(claimKey.toString()) >= 0)
								hasClaim = true;
						} catch (e) {
							console.error(`issue with claim key: ${claimKey} - ${e}`)
						}

					});
					return hasClaim;
				}
			}

			$scope.canViewUsers = $rootScope.checkClaims([SecurityKeys.UserView, SecurityKeys.UserEdit]);
			$scope.canViewRoles = $rootScope.checkClaims([SecurityKeys.RoleView, SecurityKeys.RoleEdit]);
			$scope.canViewCodeTypes = $rootScope.checkClaims([SecurityKeys.CodeTypes, SecurityKeys.Codes]);
			$scope.canExportInvoices = $rootScope.checkClaims([SecurityKeys.PeachTreeExportInvoices]);
			$scope.canExportReceipts = $rootScope.checkClaims([SecurityKeys.PeachTreeExportReceipts]);

			$scope.canViewProductShipped = $rootScope.checkClaims([
				SecurityKeys.ProductShippedView,
				SecurityKeys.ProductShippedViewTotals,
				SecurityKeys.ProductShippedSeeAllNoTotals,
				SecurityKeys.ProdcutShippedSeeAllWithTotals]);

			$scope.canViewCustomerOrders = $rootScope.checkClaims([
				SecurityKeys.CustomerOrderView,
				SecurityKeys.CustomerOrderEdit,
				SecurityKeys.CustomerOrderRevokeOrder,
				SecurityKeys.CustomerOrderExport,
				SecurityKeys.CustomerOrderCustomersSeeAll,
				SecurityKeys.CustomerOrderDomesticManagerApproval,
				SecurityKeys.CustomerOrderDomesticVPApproval,
				SecurityKeys.CustomerOrderDomesticCustomersSeeAll,
				SecurityKeys.CustomerOrderDomesticDistributorManagerApproval,
				SecurityKeys.CustomerOrderDomesticDistributorVPApproval,
				SecurityKeys.CustomerOrderDomesticDistributorSeeAll,
				SecurityKeys.CustomerOrderInternationalManagerApproval,
				SecurityKeys.CustomerOrderInternationalVPApproval,
				SecurityKeys.CustomerOrderInternationalSeeAll,
				SecurityKeys.CustomerOrderShippable,
				SecurityKeys.CustomerOrderShipRescind,
				SecurityKeys.CustomerOrderFulfillment,
				SecurityKeys.CustomerOrderFulfillmentRescind
			]);

			$scope.canViewCustomers = $rootScope.checkClaims([
				SecurityKeys.CustomerView,
				SecurityKeys.CustomerEdit,
				SecurityKeys.CustomerSeeAll,
				SecurityKeys.ExportCustomerList
			]);

			$scope.canViewVendors = $rootScope.checkClaims([
				SecurityKeys.VendorView,
				SecurityKeys.VendorEdit
			]);

			$scope.canViewProducts = $rootScope.checkClaims([
				SecurityKeys.ProductView,
				SecurityKeys.ProductEdit,
				SecurityKeys.ExportProductList,
				SecurityKeys.InventoryAdjust,
			]);

			$scope.canViewPurchaseOrders = $rootScope.checkClaims([
				SecurityKeys.PurchaseOrderView,
				SecurityKeys.PurchaseOrderDomesticView,
				SecurityKeys.PurchaseOrderDomesticDistributionView,
				SecurityKeys.PurchaseOrderInternationalView,
				SecurityKeys.PurchaseOrderEdit,
				SecurityKeys.PurchaseOrderDomesticEdit,
				SecurityKeys.PurchaseOrderDomesticDistributionEdit,
				SecurityKeys.PurchaseOrderInternationalEdit,
				SecurityKeys.PurchaseOrderReceive,
				SecurityKeys.PurchaseOrderDomesticVPApproval,
				SecurityKeys.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeys.PurchaseOrderInternationalVPApproval
			]);

			$scope.canAdjustPrice = $rootScope.checkClaims([
				SecurityKeys.PriceAdjustmentEdit
			]);

			$scope.canViewBackOrders = $rootScope.checkClaims([
				SecurityKeys.BackorderSeeAll,
				SecurityKeys.BackorderView,
				SecurityKeys.BackorderViewWithTotals,
				SecurityKeys.BackorderSeeAllNoTotals
			]);

			$scope.canViewArchive = $rootScope.checkClaims([
				SecurityKeys.ArchiveView,
				SecurityKeys.ArchiveSeeAll,
				SecurityKeys.ArchiveSeeAllNoTotals,
				SecurityKeys.ArchiveViewWithTotals]);

			$scope.canViewProductActivity = $rootScope.checkClaims([SecurityKeys.ProductActivityView]);
		}

		$scope.callPreviousPeachTreeInvoicesFunc = function () {
			var modal = $uibModal.open({
				templateUrl: "PreviousPeachTreeInvoices.html",
				backdrop: "static",
				keyboard: false,
				resolve: {
					data: function () {
						return {

						}
					}, service: peachTreeService
				},
				controller: [
					"$scope", "$uibModalInstance", "data", "service",

					function ($scope, $uibModalInstance, data, service) {
						Object.assign($scope, data);
						$scope.modalLoading = true;

						$scope.GetPreviouseInvoiceList = function GetPreviouseInvoiceList(topResults) {
							service.getPreviousInvoiceList({ "TopResults": topResults }, function (response) {
								$scope.previousInvoices = response.data;
								$scope.modalLoading = false;

							}, function (err) { alert(err.data); $scope.modalLoading = false; });
						}

						$scope.close = function () {
							$uibModalInstance.dismiss();
						}

						$scope.UpdateExportList = function (results) {
							GetPreviouseInvoiceList(results);
						}

						$scope.getPreviousInvoicesByDate = function(){
							const minDate = $('#start_date').val();
							const maxDate = $('#end_date').val();

							if (minDate === "" || maxDate === "") {
								alert("Please select start and end date");
                                return;
							}
							if (maxDate < minDate) {
								alert("End date must be greater than start date");
								return;
							}

							$scope.modalLoading = true;

							service.getPreviousInvoicesByDate({ "StartDate": new Date(minDate), "EndDate": new Date(maxDate) }, function (response) {
								var a = document.createElement("a");
								a.href = URL.createObjectURL(new Blob([response.data], { type: "application/excel" }));
								a.target = "_blank";
								a.download = `InvoiceExport_${new Date().toISOString()}.csv`

								document.body.appendChild(a);
								a.click();
								document.body.removeChild(a);
								URL.revokeObjectURL(a.href);
								$scope.modalLoading = false;
							}, function (err) { alert(err.data); $scope.modalLoading = false; });
						}

						$scope.GetPreviouseInvoiceList(30);
					}
				]
			});
		};
}]);