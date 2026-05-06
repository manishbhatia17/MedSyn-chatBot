$app.controller("productListController", [
	"$scope", "$rootScope", "productService", "$uibModal",
	function ($scope, $rootScope, productService, $uibModal) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.ProductView]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.ProductEdit]);
		$scope.canExport = $rootScope.checkClaims([SecurityKeys.ExportProductList]);
		$scope.canAdjustInventory = $rootScope.checkClaims([SecurityKeys.InventoryAdjust]);
		$scope.canEditPurchaseOrder = $rootScope.checkClaims([SecurityKeys.PurchaseOrderEdit]);

		$scope.table = {
			idField: "productID",
			showEdit: $scope.canEdit || $scope.canView,
			hideAdd: !$scope.canEdit,
			hideUpload: true,
			hidePriceImport: true,
			exportExcelFn: $scope.canExport ? exportProductList : null,
			dataColumns: [
				{ title: "Name", field: "productName" },
				{ title: "Prod ID", field: "productCustomID" },
				{ title: "U/M", field: "unitOfMeasureCodeID" },
				{ title: "On Hand", field: "onHandQuantity"},
				{ title: "Committed", field: "committedQuantity" },
				{ title: "PO", field: "poQuantity"},
				{ title: "Net Qty", field: "netQuantity" },
				{ title: "R/O Pnt", field: "reorderPoint" },
				{ title: "R/O Qty", field: "reorderQuantity" },
				{ title: "Pri Vendor", field: "primaryVendorID" }//,
				//{ title: "Cost", field: "cost" }
			],
			actionColumns: [
				{ title: "Order", func: order, disabled: (product) => product.isDiscontinued, hide: !$scope.canEditPurchaseOrder },
				{ title: "Adjust", func: adjust, hide: !$scope.canAdjustInventory }
			],
			additionalFilters: [
				{
					type     : "checkbox",
					parameter: "showDiscontinued",
					label    : { position: "right", text: "Discontinued" },
				}
			]
		};

		function order(productID, product) {
			location.href = `/purchaseOrder/details/0?productID=${productID}&priVendorID=${product.priVendorID}`;
		}

		function adjust(productID) {
			var modal = $uibModal.open({
				templateUrl: "ProductAdjustInventoryModal.html",
				backdrop: "static",
				keyboard: false,
				controller: [
					"$scope", "$uibModalInstance",
					function ($scope, $uibModalInstance) {
						productService.getProductInventoryAdjustments(productID,
							(res) => {
								var {reasonCodes, historyList, ...other} = res.data;
								Object.assign($scope, other);

								$scope.adjustment  = { productID: productID };
								$scope.reasonCodes = reasonCodes;
								$scope.historylist = historyList;

								$scope.reasonCodesDict = reasonCodes.reduce((agg, cur) => {
									agg[cur.value] = cur.text;
									return agg;
								}, {});
							}
						);

						$scope.save = function () {
							if($scope.form.$invalid) {
								$rootScope.showInvalidFields($scope.form);
								return;
							}
							productService.saveProductAdjustment($scope.adjustment, () => {
								$uibModalInstance.close();
							});
						};

						$scope.cancel = function () {
							if($scope.form.$dirty) {
								var save = window.confirm("You have unsaved changes. Click OK to save, or Cancel to discard.")
								if(save) {
									$scope.save();
									return;
								}
							}
							$uibModalInstance.dismiss();
						};
					}
				]
			});

			modal.result.then(() => {
				$scope.table.refresh();
			}, () => {
				// do nothing, just don't error
			});
		};

		function exportProductList(tblScope) {
			productService.exportProducts(tblScope.searchCriteria, null, (res) => {
				var a = document.createElement("a");
				a.href = URL.createObjectURL(new Blob([res.data], {type: "application/excel"}));
				a.target = "_blank";
				a.download = "ProductList.xlsx"

				document.body.appendChild(a);
				a.click();
				document.body.removeChild(a);
				URL.revokeObjectURL(a.href);
			});
		}
	}
]);