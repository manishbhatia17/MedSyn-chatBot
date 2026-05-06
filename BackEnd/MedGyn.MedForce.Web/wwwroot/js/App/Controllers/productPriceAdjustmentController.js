$app.controller("productPriceAdjustmentController", [
	"$scope", "$rootScope", "productPriceAdjustmentService",
	function ($scope, $rootScope, productPriceAdjustmentService) {
		
		$scope.canView = $rootScope.checkClaims([SecurityKeys.ProductView]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.ProductEdit]);
		$scope.canExport = $rootScope.checkClaims([SecurityKeys.ExportProductList]);
		if(!$scope.canEdit)
			return;
		
		$scope.table = {
			idField: "productID",
			hideAdd: true,
			hideUpload: true,
			hidePriceImport: false,
			exportExcelFn: exportList,
			dataColumns: [
				{ title: "Prod ID", field: "productCustomID", required: true, editable: false },
				{ title: "Product Description", field: "description", required: true, editable: false },
				{ title: "Domestic List Price", field: "priceDomesticList", editable: true, type: "number" },
				{ title: "Domestic Premier Price", field: "priceDomesticPremier", editable: true, type: "number" },
				{ title: "Domestic Afaxsys Price", field: "priceDomesticAfaxys", editable: true, type: "number" },
				{ title: "Main Distributor Price", field: "priceMainDistributor", editable: true, type: "number" },
				{ title: "Domestic Distribution Price", field: "priceDomesticDistribution", editable: true, type: "number" },
				{ title: "International Distribution Price", field: "priceInternationalDistribution", editable: true, type: "number" },
				{ title: "Cost", field: "cost", editable: true, type: "number" },
			],
			additionalFilters: [
				{
					type: "checkbox",
					parameter: "showDiscontinued",
					label: { position: "right", text: "Discontinued" },
				}
			]
		};

		$scope.save = function(data, form, refreshFunc) {
			productPriceAdjustmentService.saveProductPricingInformation(data, (res) => {
				form.$setPristine();
				refreshFunc(res.data);
			});
		}

		function exportList(tblScope) {
			console.log(tblScope);
			productPriceAdjustmentService.exportProducts(tblScope.searchCriteria, null, (res) => {
				console.log(res);
				var a = document.createElement("a");
				a.href = URL.createObjectURL(new Blob([res.data], { type: "application/excel" }));
				a.target = "_blank";
				a.download = "ProductPrice.xlsx";
				document.body.appendChild(a);
				a.click();
				document.body.removeChild(a);
				URL.revokeObjectURL(a.href);
			});
		}
	}
]);