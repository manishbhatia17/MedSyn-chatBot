// Contains list of Products
$app.controller("productDetailsController", [
	"$scope", "$rootScope", "productService",
	function ($scope, $rootScope, productService) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.ProductView]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.ProductEdit]);
		$scope.costVisible = $rootScope.checkClaims([SecurityKeys.CostVisible]);

		$scope.save = function () {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			productService.saveProduct($scope.product, (res) => {
				if(res.data.success) {
					$scope.form.$setPristine();
					location.href = "/product";
				} else {
					if(res.data.errorMessage === "DUP_ID") {
						$scope.form.productCustomID.$setValidity('unique', false);
					} else {
						alert("Error saving Product. Check console for details.");
						console.error(res.data.errorMessage, res.data.fullError);
					}
				}
			});
		};

		$scope.clearInvalid = function() {
			$scope.form.productCustomID.$setValidity('unique', true);
		}

		$scope.changeInternationOnly = function() {
			if($scope.product.internationalOnly) {
				$scope.product.priceDomesticList         = null;
				$scope.product.priceDomesticDistribution = null;
			}
		}

		$scope.viewOnly = function() {
			return !$scope.canEdit;
		}

		$scope.init = function (productID) {
			if(!$scope.canEdit && !$scope.canView)
				return;

			productService.getProductDetails(productID,
				(res) => {
					$scope.product                = res.data.product;
					$scope.vendors                = res.data.vendors;
					$scope.umCodes                = res.data.umCodes;
					$scope.shipWeightUnitCodes    = res.data.shipWeightUnitCodes;
					$scope.shipDimensionUnitCodes = res.data.shipDimensionUnitCodes;
				}
			)

			$("#main-image-upload").on("change", () => {
				var file = event.target.files[0];
				if(!file) {
					return;
				}
				var reader = new FileReader();

				reader.onload = function (e) {
					$("#primaryImage").attr("src", e.target.result);
					$scope.product.primaryImageURI = e.target.result;
					$scope.$apply();
				}

				reader.readAsDataURL(file);
			});

			$("#extra-image-upload").on("change", () => {
				var file = event.target.files[0];
				var reader = new FileReader();

				reader.onload = function (e) {
					var imgArray = Array.from($(".extra-images img"));
					for(var img of imgArray) {
						var propName = `${img.id}URI`;
						if(!$scope.product[propName]) {
							img.src = e.target.result;
							$scope.product[propName] = e.target.result;
							break;
						}
					}
					$scope.$apply();
				}

				reader.readAsDataURL(file);
			});
		};
	}
]);