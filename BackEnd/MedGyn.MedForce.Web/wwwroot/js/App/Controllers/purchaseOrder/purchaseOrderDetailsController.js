$app.controller("purchaseOrderDetailsController", [
	"$scope", "$rootScope", "purchaseOrderService", "vendorService", "$uibModal",
	function ($scope, $rootScope, purchaseOrderService, vendorService, $uibModal) {
		var me = this;

		$scope.today = new Date();

		$scope.save = function (submit) {
			if ($scope.form.$invalid && !($scope.isDoNotReceiveEnabled() && $scope.purchaseOrder.isDoNotReceive)) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}
			$scope.isSaving = true;
			purchaseOrderService.savePurchaseOrder($scope.purchaseOrder, submit, () => {
				$scope.form.$setPristine();
				var po = $scope.purchaseOrder;
				if(po.submitDate && !po.approvedBy) {
					$scope.isSaving = false;
					return;
				}
				location.href = "/purchaseOrder";
			});
		};

		$scope.viewOnly = function () {
			return !($scope.canEdit || $scope.isVP );
		};

		$scope.isDoNotReceiveEnabled = function () {
			return $scope.canDoNotReceive && $scope.purchaseOrder && $scope.purchaseOrder.approvedBy;
		}

		$scope.init = function(purchaseOrderID) {
			$scope.canEdit         = $rootScope.checkClaims([
				SecurityKeys.PurchaseOrderEdit,
				SecurityKeys.PurchaseOrderDomesticEdit,
				SecurityKeys.PurchaseOrderDomesticDistributionEdit,
				SecurityKeys.PurchaseOrderInternationalEdit
			]);
			$scope.canReceive      = $rootScope.checkClaims([SecurityKeys.PurchaseOrderReceive]);
			$scope.canDoNotReceive = $rootScope.checkClaims([SecurityKeys.PurchaseOrderDoNotReceiveFlag]);
			$scope.purchaseOrderID = purchaseOrderID;
			var urlParams              = new URLSearchParams(location.search);
			var productID              = urlParams.get("productID")
			var priVendorID            = urlParams.get("priVendorID")

			purchaseOrderService.getPurchaseOrderDetails(purchaseOrderID, { productID: productID, priVendorID: priVendorID },
				(res) => {
					$scope.purchaseOrder = res.data.purchaseOrder;
					$scope.vendors       = res.data.vendors;
					if($scope.purchaseOrder.purchaseOrderID > 0) {
						$scope.canEdit = $rootScope.checkClaims([SecurityKeys.PurchaseOrderEdit]);
						if($scope.purchaseOrder.isDomestic){
							$scope.canEdit = $scope.canEdit || $rootScope.checkClaims([SecurityKeys.PurchaseOrderDomesticEdit]);
							$scope.isVP = $rootScope.checkClaims([SecurityKeys.PurchaseOrderDomesticVPApproval]);
						}
						if($scope.purchaseOrder.isDomesticDistribution){
							$scope.canEdit = $scope.canEdit || $rootScope.checkClaims([SecurityKeys.PurchaseOrderDomesticDistributionEdit]);
							$scope.isVP = $rootScope.checkClaims([SecurityKeys.PurchaseOrderDomesticDistributionVPApproval]);
						}
						if($scope.purchaseOrder.isInternational){
							$scope.canEdit = $scope.canEdit || $rootScope.checkClaims([SecurityKeys.PurchaseOrderInternationalEdit]);
							$scope.isVP = $rootScope.checkClaims([SecurityKeys.PurchaseOrderInternationalVPApproval]);
						}
					}

					if($scope.purchaseOrder.expectedDate)
						$scope.purchaseOrder.expectedDate = new Date($scope.purchaseOrder.expectedDate);

					$scope.shipCompanies    = res.data.shipCompanies;
					$scope.shippingAccounts = res.data.shippingAccounts;
					$scope.umCodes          = res.data.umCodes;
					$scope.vendorTaxes      = res.data.vendorTaxes;

					$scope.vendorDict = res.data.vendors.reduce((agg, cur) => {
						agg[cur.value] = cur.text;
						return agg;
					}, {});

					//TODO: use enum values
					$scope.shipMethodCodes = {
						1: res.data.fedExShipMethodCodes,
						2: res.data.upsShipMethodCodes,
					};

					if($scope.purchaseOrder.vendorID) {
						$scope.changeVendor();
					}

					$scope.purchaseOrder.products.forEach((p, i) => {
						$scope.changeProduct(i);
					});

					if(!$scope.purchaseOrder.products.length) {
						$scope.addProduct();
					}
				}
			)
		}

		$scope.changeVendor = function() {
			vendorService.getProductsForVendor($scope.purchaseOrder.vendorID,
				(res) => {
					$scope.products = res.data;
				}
			)
		}

		$scope.changeProduct = function(idx) {
			purchaseOrderService.getPurchaseOrderHistoryForProduct($scope.purchaseOrder.products[idx].productID,
				(res) => {
					$scope.purchaseOrder.products[idx].meta                = res.data;
					$scope.purchaseOrder.products[idx].unitOfMeasureCodeID = res.data.umCodeID;
				}
			)
		}

		$scope.doNotReceiveChanged = function () {
			if (!$scope.purchaseOrder.isDoNotReceive)
				$scope.purchaseOrder.doNotReceiveReason = '';
		}

		$scope.addProduct = function() {
			$scope.purchaseOrder.products.push({
				purchaseOrderProductID: -$scope.purchaseOrder.products.length - 1
			});
		}

		$scope.deleteProduct = function(idx) {
			var product = $scope.purchaseOrder.products[idx];

			if(product.purchaseOrderProductID > 0) {
				product.markedForDelete = true;
				$scope.form.$setDirty();
			} else {
				$scope.purchaseOrder.products.splice(idx, 1);
			}
		}

		$scope.approve = function() {
			if(!$scope.isVP)
				return;
			purchaseOrderService.approvePurchaseOrder($scope.purchaseOrderID, (res) => {
				if(res.data) {
					$scope.form.$setPristine();
					location.href = "/purchaseOrder";
				} else {
					alert("Could not approve purchase order")
				}
			}, (err) => alert("Could not approve purchase order"));
		}

		$scope.totalProducts = function() {
			return $scope.purchaseOrder?.products.length;
		}

		$scope.totalItems = function() {
			return $scope.purchaseOrder?.products.reduce((a, b) => a + (b.quantity || 0), 0)
		}

		$scope.totalPrice = function() {
			return $scope.purchaseOrder?.products.reduce((a, b) => a + ((b.quantity * b.price) || 0), 0)
		}

		$scope.tax = function () {
			if($scope.purchaseOrder?.vendorID) {
				var tax = $scope.vendorTaxes[$scope.purchaseOrder.vendorID];
				return $scope.totalPrice() * (tax / 100);
			}
		}

		$scope.export = function() {
			var modal = $uibModal.open({
				templateUrl: "SendingPOEmail.html",
				keyboard: false,
				resolve: {data: () => {}},
				controller: [
					"$scope", "$uibModalInstance", "data",
					function($scope, $uibModalInstance, data) {
						$scope.close = function() {
							$uibModalInstance.dismiss();
						}
					}
				]
			});

			purchaseOrderService.sendPurchaseOrderReport(
				$scope.purchaseOrder.purchaseOrderID,
				() => modal.dismiss(),
				(res) => {
					modal.dismiss();
					alert(res?.data?.errorMessage ?? "Failed to send email to Vendor");
				});
		}
	}
]);