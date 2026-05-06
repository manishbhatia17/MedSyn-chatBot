$app.controller("vendorDetailsController", [
	"$scope", "$rootScope", "vendorService",
	function ($scope, $rootScope, vendorService) {

		$scope.save = function () {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			vendorService.saveVendor($scope.vendor, (res) => {
				if(res.data.success) {
					$scope.form.$setPristine();
					location.href = "/vendor";
				} else {
					if(res.data.errorMessage === "DUP_ID") {
						$scope.form.vendorCustomID.$setValidity('unique', false);
					} else {
						alert("Error saving Vendor. Check console for details.");
						console.error(res.data.errorMessage, res.data.fullError);
					}
				}
			});
		};

		$scope.viewOnly = function () {
			return !$scope.canEdit;
		}

		$scope.clearInvalid = function() {
			$scope.form.vendorCustomID.$setValidity('unique', true);
		}

		$scope.init = function (vendorID) {
			$scope.canView = $rootScope.checkClaims([SecurityKeys.VendorView]);
			$scope.canEdit = $rootScope.checkClaims([SecurityKeys.VendorEdit]);
			if(!$scope.canEdit && !$scope.canView)
				return;

			vendorService.getVendorDetails(vendorID,
				(res) => {
					$scope.vendor                      = res.data.vendor;
					$scope.stateCodes                  = res.data.stateCodes;
					$scope.countryCodes                = res.data.countryCodes;
					$scope.glPurchaseAccountCodes      = res.data.glPurchaseAccountNumberCodes;
					$scope.glFreightChargeAccountCodes = res.data.glFreightChargeAccountNumberCodes;
					$scope.glAccountsPayableCodes      = res.data.glAccountsPayableNumberCodes;
					$scope.vendorStatusCodes           = res.data.vendorStatusCodes;
				}
			)
		}
	}
]);