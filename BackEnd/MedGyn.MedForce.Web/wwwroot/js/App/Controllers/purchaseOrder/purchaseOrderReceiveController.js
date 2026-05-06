$app.controller("purchaseOrderReceiveController", [
	"$scope", "$rootScope", "$compile", "purchaseOrderService",
	function ($scope, $rootScope, $compile, purchaseOrderService) {
		var me = this;
		$scope.canReceive = $rootScope.checkClaims([SecurityKeys.PurchaseOrderReceive]);
		if(!$scope.canReceive)
			return;
		$scope.table = {
			idField: "purchaseOrderProductID",
			hideSearch: true,
			hideAdd: true,
			hideSave: true,
			noPagination: true,
			sorting: false,
			dataColumns: [
				{ title: "Prod ID", field: "productCustomID" },
				{ title: "Name", field: "productName" },
				{ title: "U/M", field: "unitOfMeasure" },
				{ title: "Order Qty", field: "orderQuantity" },
				{ title: "Qty to Rcv", field: "quantityToReceive" },
				{ title: "Received", field: "quantityReceived", editable: (field, row) => row.quantityToReceive > 0 , type: "number" },
				{ title: "Lot/Serial Numbers", field: "serialNumbers", editable: (field, row) => row.quantityToReceive > 0, required: (field, row) => !!row.quantityReceived },
			]
		}

		$scope.init = function(purchaseOrderID) {
			me.purchaseOrderID = purchaseOrderID;
			purchaseOrderService.getPurchaseOrderReceive(me.purchaseOrderID, null, (res) => {
				$scope.purchaseOrder = res.data;
				$scope.table.data    = $scope.purchaseOrder.products.map((p) => {
					// do not care about values from last receipt
					p.quantityReceived = undefined;
					p.serialNumbers = undefined;

					return p;
				});
				$scope.purchaseOrder.shippingFrom = [
					$scope.purchaseOrder.fromCity,
					$scope.purchaseOrder.fromState,
					$scope.purchaseOrder.fromCountry,
				].filter((l) => l).join(", ");

				setTimeout(() => {
					for(var p of $scope.purchaseOrder.products) {
						var receivedInputID = `quantityReceived_${p.purchaseOrderProductID}`;
						var receivedInput = angular.element(`#${receivedInputID}`);
						receivedInput.attr({"ng-min": 0, "ng-max": p.orderQuantity});
						$compile(receivedInput)(receivedInput.scope());
						angular.element(`#${receivedInputID}_errors`).scope().maxMsg = "Received qty exceeds qty to rcv.";

						var serialNumbersInputID = `serialNumbers_${p.purchaseOrderProductID}`;
						var serialNumbersInput = angular.element(`#${serialNumbersInputID}`);
						serialNumbersInput.attr({"ng-maxlength": 200, "ng-pattern": /^[a-z0-9,;:.\s-]*$/i});
						$compile(serialNumbersInput)(serialNumbersInput.scope());
						angular.element(`#${serialNumbersInputID}_errors`).scope().maxlengthMsg = "Serial Numbers exceed 200 characters.";
					}
				}, 100)
			});
		}

		$scope.save = function() {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			if(!$scope.purchaseOrder.products.some((p) => p.quantityReceived)) {
				return alert("No items have been received");
			}

			purchaseOrderService.receiptComplete(
				$scope.purchaseOrder.purchaseOrderID,
				$scope.purchaseOrder.products,
				(res) => {
					$scope.form.$setPristine();
					location.href = "/PurchaseOrder";
				});
		}
	}
]);