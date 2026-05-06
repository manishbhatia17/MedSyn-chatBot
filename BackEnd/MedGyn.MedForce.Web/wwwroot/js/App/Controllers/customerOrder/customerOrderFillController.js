$app.controller("customerOrderFillController", [
	"$scope", "$rootScope", "$compile", "customerOrderService",
	function ($scope, $rootScope, $compile, customerOrderService) {
		var me = this;
		
		$scope.table = {
			idField: "customerOrderProductID",
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
				{ title: "Qty to Ship", field: "quantityToShip" },
				{ title: "Qty Packed", field: "quantityPacked", editable: (field, row) => row.quantityToShip > 0 && $scope.isEditable, type: "number" },
				{ title: "Lot/Serial Numbers", field: "serialNumbers", editable: (field, row) => row.quantityToShip > 0 && $scope.isEditable, required: (field, row) => !!row.quantityPacked },
			]
		}

		$scope.processing = false;
		$scope.isEditable = true;

		$scope.init = function (customerOrderID) {
			
			me.customerOrderID = customerOrderID;
			var urlParams  = new URLSearchParams(location.search);
			me.boxID = parseFloat(urlParams.get("boxID"));

			customerOrderService.getCustomerOrderFill(customerOrderID, { boxID: me.boxID },
				(res) => {
					$scope.customerOrder = res.data;
					$scope.table.data    = $scope.customerOrder.products;
					$scope.isVaried      = res.data.boxes?.length > 0;
					me.boxes             = res.data.boxes;

					$scope.isFirstBox = me.boxes.indexOf(me.boxID) === 0 || me.boxes.length === 0;
					//first check if the user is the assigned user
					$scope.isEditable = $scope.customerOrder.isFilling == true && $scope.customerOrder.filledById > 0 && $scope.customerOrder.filledById == $rootScope.userId ? true :
						//next check if there is an assigned user to handle legacy filling items
						$scope.customerOrder.filledById <= 0 ? true :
						//verify it is in filling
						$scope.customerOrder.isFilling == false ? true : false;

					setTimeout(() => {
						
						for(var p of $scope.customerOrder.products) {
							var id = `quantityPacked_${p.customerOrderProductID}`;

							var el = angular.element(`#${id}`);
							el.attr({ "ng-max": p.quantityToShip, "ng-min": 0 });
							$compile(el)(el.scope());

							angular.element(`#${id}_errors`).scope().maxMsg = "Packed qty exceeds qty to ship";
						}
					}, 100)
				}
			)
		}

		$scope.nextBox = function () {
			
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			if(me.boxID) {
				var goToNextBox = function(){
					var idx = me.boxes.indexOf(me.boxID);
					if(idx === me.boxes.length - 1){
						location.href = `/CustomerOrder/Fill/${me.customerOrderID}`;
					} else {
						var newBoxID = me.boxes[idx + 1]
						location.href = `/CustomerOrder/Fill/${me.customerOrderID}?boxID=${newBoxID}`;
					}
				}

				if(!$scope.form.$dirty) {
					goToNextBox();
				} else {
					customerOrderService.updateBox(me.customerOrderID, me.boxID, $scope.customerOrder,
						(res) => {
							if(res.data.success) {
								$scope.form.$setPristine();
								goToNextBox();
							} else {
								console.error(res.data.errorMessage);
								alert("There was an error saving the box");
							}
						}
					)
				}
			} else {
				customerOrderService.addBox(me.customerOrderID, $scope.customerOrder,
					(res) => {
						if(res.data.success) {
							$scope.form.$setPristine();
							location.href = location.href;
						} else {
							console.error(res.data.errorMessage);
							alert("There was an error saving the box");
						}
					}
				)
			}
		}

		$scope.prevBox = function() {
			var idx = me.boxes.length;
			if(me.boxID)
				idx = me.boxes.indexOf(me.boxID);

			var newBoxID = me.boxes[idx - 1]
			location.href = `/CustomerOrder/Fill/${me.customerOrderID}?boxID=${newBoxID}`;
		}

		$scope.fillComplete = function () {
			
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			if(!me.boxes.length && !$scope.customerOrder.products.some(x => x.quantityPacked))
			{
				alert("No items have been packed")
				return;
			}
			$scope.processing = true;
			customerOrderService.fillComplete(me.customerOrderID, $scope.customerOrder,
				(res) => {
					$scope.form.$setPristine();
					window.open(`/CustomerOrder/PackingSlip/${res.data.customerOrderShipmentID}/${$scope.customerOrder.generateMultiplePackingSlip}`, "_blank");
					location.href = "/CustomerOrder";
				},
				(res) => {
					console.error(res.data.errorMessage);
					alert("There was an completing the fill: " + res.data.errorMessage);
					$scope.processing = false;
				}
			)
		}

		$scope.isOrderDone = function() {
			if(!$scope.customerOrder?.products)
				return false;

			var [totalQuantityToShip, curPacked] = $scope.customerOrder.products.reduce((agg, cur) => {
				agg[0] += cur.quantityToShip;
				agg[1] += cur.quantityPacked || 0;
				return agg;
			}, [0, 0])
			return !(totalQuantityToShip - curPacked);
		}
	}
]);