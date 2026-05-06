$app.controller("customerOrderShipController", [
	"$scope", "$rootScope", "$timeout", "customerOrderService",
	function ($scope, $rootScope, $timeout, customerOrderService) {
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
				{ title: "Packed", field: "quantityPacked" },
				{ title: "Lot/Serial Numbers", field: "serialNumbers" },
			]
		}

		$scope.customerOrderID;

		//need to check when shipping forrm
		$scope.requiresFinancingApproval = false;
		$scope.financeApprover = $rootScope.checkClaims([SecurityKeys.CustomerOrderFinanceApprover]);
		$scope.financeApproved = false;


		$scope.approveFinancing = function () {

			customerOrderService.approveFinancing($scope.customerOrderID, null,() => {
				$scope.init($scope.customerOrderID);
			});
        }

		$scope.save = function (submit) {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			customerOrderService.saveCustomerOrder($scope.customerOrder, submit, () => {
				$scope.form.$setPristine();
				var co = $scope.customerOrder;
				if(co.submitDate && (!co.mgApprovedBy || !co.vpApprovedBy))
					return;
				location.href = "/customerOrder";
			});
		};

		$scope.init = function (customerOrderID) {
			
			me.customerOrderID = customerOrderID;
			$scope.customerOrderID = customerOrderID;
			var urlParams  = new URLSearchParams(location.search);
			me.shipmentID = parseFloat(urlParams.get("shipmentID"));
			me.boxID = parseFloat(urlParams.get("boxID"));

			customerOrderService.getCustomerOrderShip(customerOrderID, me.shipmentID, { boxID: me.boxID },
				(res) => {
					const customerIDPrefix = res.data.fill.customerCustomID.substring(0, 2);
					const usaCountryCode = 3;
					if (customerIDPrefix === "3-" || customerIDPrefix === "4-" || res.data.billCountry !== usaCountryCode) {
						$scope.requiresFinancingApproval = true;
						$scope.financeApproved = res.data.fill.financingApproved;;
					}
					else {
						$scope.requiresFinancingApproval = false;
						$scope.financeApproved = true;
                    }

					var currentBoxId = res.data.shipmentBox.customerOrderShipmentBoxID;
					res.data.fill.boxes.forEach((currentValue, index) => {
						customerOrderService.getCustomerOrderShip(customerOrderID, me.shipmentID, { boxID: currentValue },
							(response) => {
								createTableOfFilledBoxes(response.data);
								showTableOfFilledBoxes(currentBoxId);
							}
						)
					});
					

					$scope.customerOrder      = res.data.fill;
					$scope.shipment           = res.data.shipment;
					$scope.shipmentBox        = res.data.shipmentBox;
					$scope.boxes              = res.data.fill.boxes;
					$scope.weightUnitCodes    = res.data.weightUnitCodes;
					$scope.dimensionUnitCodes = res.data.dimensionUnitCodes
					$scope.shipCompanyCodes   = res.data.shipCompanyCodes;
					$scope.otherBoxesDone = res.data.otherBoxesDone;
					
					

					me.fedExShipMethodCodes      = res.data.fedExShipMethodCodes;
					me.upsShipMethodCodes        = res.data.upsShipMethodCodes;
					me.otherShipMethodCodes      = res.data.otherShipMethodCodes;
					me.accountNumbers            = res.data.accountNumbers;

					$scope.fedExCodeID       = res.data.fedExCodeID;
					$scope.fedExMedGynCodeID = res.data.fedExMedGynCodeID
					$scope.upsCodeID         = res.data.upsCodeID;
					$scope.upsMedGynCodeID   = res.data.upsMedGynCodeID;
					$scope.upsFreightCodeID  = res.data.upsFreightCodeID;

					me.boxID = $scope.shipmentBox.customerOrderShipmentBoxID;
					if($scope.shipmentBox.isFormPrefilled) {
						$scope.form.$setDirty();
						$scope.shipmentBox.length = "";
						$scope.shipmentBox.width = "";
						$scope.shipmentBox.depth = "";
						$scope.shipmentBox.weight = "";
						$scope.shipmentBox.dimensionUnitCodeID = "";
						$scope.shipmentBox.weightUnitCodeID = "";
					}

					$scope.isFirstBox = $scope.boxes.indexOf(me.boxID) === 0
					$scope.isLastBox  = $scope.boxes.indexOf(me.boxID) === $scope.boxes.length - 1

					$scope.table.data = $scope.customerOrder.products;

					$scope.changeShipCompany(true);

					$scope.shipmentBox.dimensionUnitCodeID = $scope.shipmentBox.dimensionUnitCodeID ? $scope.shipmentBox.dimensionUnitCodeID : 469;
					$scope.shipmentBox.weightUnitCodeID = $scope.shipmentBox.weightUnitCodeID ? $scope.shipmentBox.weightUnitCodeID : 10;
				}
			)
		}

		$scope.hasAllShippingInfo = function () {
			const validForm = $scope.otherBoxesDone && $scope.form.$valid && $scope.shipment.shipCompanyType && $scope.shipment.shipMethodCodeID;

			if (validForm)
				return true;
		}

		$scope.nextBox = function() {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			var goToNextBox = function(){
				var idx = $scope.boxes.indexOf(me.boxID);
				if(idx === $scope.boxes.length - 1){
					location.href = `/CustomerOrder/Ship/${me.customerOrderID}?shipmentID=${me.shipmentID}`;
				} else {
					var newBoxID = $scope.boxes[idx + 1]
					location.href = `/CustomerOrder/Ship/${me.customerOrderID}?shipmentID=${me.shipmentID}&boxID=${newBoxID}`;
				}
			}

			if(!$scope.form.$dirty) {
				goToNextBox();
			} else {
				customerOrderService.updateBoxDims(me.customerOrderID, me.boxID,
					{ shipmentBox: $scope.shipmentBox, shipment: $scope.shipment },
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
		}

		$scope.addBox = function () {
			if ($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			var goToNewBox = function () {
				customerOrderService.addShippingBox(me.customerOrderID, me.shipmentID,
					(res) => {
						if (res.data.success) {
							console.log(res);
							$scope.form.$setPristine();
							location.href = location.href = `/CustomerOrder/Ship/${me.customerOrderID}?shipmentID=${me.shipmentID}&boxID=${res.data.payload.customerOrderShipmentBoxID}`;;

						} else {
							console.error(res.data.errorMessage);
							alert("There was an error saving the box");
						}
					}
				);
			}

			if (!$scope.form.$dirty) {
				goToNewBox();
			} else {
				customerOrderService.updateBoxDims(me.customerOrderID, me.boxID,
					{ shipmentBox: $scope.shipmentBox, shipment: $scope.shipment },
					(res) => {
						if (res.data.success) {
							$scope.form.$setPristine();
							goToNewBox();
						} else {
							console.error(res.data.errorMessage);
							alert("There was an error saving the box");
						}
					}
				)
			}

		}

		$scope.prevBox = function() {
			var idx = $scope.boxes.indexOf(me.boxID);

			var newBoxID = $scope.boxes[idx - 1]
			location.href = `/CustomerOrder/Ship/${me.customerOrderID}?shipmentID=${me.shipmentID}&boxID=${newBoxID}`;
		}

		$scope.removeBox = function () {
			var idx = $scope.boxes.indexOf(me.boxID);

			if (idx <= 0) {
				alert("You cannot remove the first box");
				return false;
			}

			if (confirm("Are you sure you want to remove this box?")) {

				var newBoxID = $scope.boxes[idx - 1];
				customerOrderService.removeShippingBox(me.customerOrderID, me.shipmentID, me.boxID, (res) => {
					if (res.data.success) {
						console.log(res);
						$scope.form.$setPristine();
						location.href = `/CustomerOrder/Ship/${me.customerOrderID}?shipmentID=${me.shipmentID}&boxID=${newBoxID}`;

					} else {
						console.error(res.data.errorMessage);
						alert("There was an error saving the box");
					}
				});
			}				
		}

		$scope.shipmentComplete = function(){
			$scope.isCompletingShipment = true;
			// give the form a chance to update the ng-required's;
			$timeout(() => completeShipment(), 10);
		}

		$scope.changeShipCompany = function(initial){
			if ($scope.shipment.shipCompanyType === $scope.fedExCodeID || $scope.shipment.shipCompanyType === $scope.fedExMedGynCodeID)
				$scope.shipChoiceCodes = me.fedExShipMethodCodes;
			else if ($scope.shipment.shipCompanyType === $scope.upsCodeID || $scope.shipment.shipCompanyType === $scope.upsMedGynCodeID)
				$scope.shipChoiceCodes = me.upsShipMethodCodes;
			else
				$scope.shipChoiceCodes = me.otherShipMethodCodes;
			if(!initial)
				$scope.shipment.shipAccountNumber = me.accountNumbers[$scope.shipment.shipCompanyType];
		}

		$scope.getRateQuote = function() {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			$scope.gettingQuote = true;
			customerOrderService.getRateQuote(me.customerOrderID, me.shipmentID, $scope.shipmentBox,
				{ shippingCompany: $scope.shipment.shipCompanyType, shippingMethod: $scope.shipment.shipMethodCodeID },
				(res) => {
					$scope.gettingQuote = false;
					var rate = parseFloat(res.data);
					$scope.rateQuote = isNaN(rate) ? null : rate;
					if(isNaN(rate) && res.data) {
						alert("Error while fetching rate")
						console.error(res.data);
					}
				},
				(res) => console.log(res));
		}

		$scope.useRateQuote = function() {
			if(!isNaN($scope.rateQuote)) {
				$scope.shipment.shippingCharge = parseFloat($scope.rateQuote.toFixed(2));
				$scope.form.shippingCharge.$setDirty();
			}
		}

		$scope.gotRateQuote = function() {
			return !isNaN($scope.rateQuote);
		}

		$scope.rescind = function(){
			customerOrderService.rescind();
		}

		$scope.updateBoxDetails = function () {
			var shipmentBox = $scope.shipmentBox;
			shipmentBox.isFormPrefilled = false;
			UpdateBoxDetails(shipmentBox);
        }

		function completeShipment() {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}
			
			$scope.completingShipment = true;
			customerOrderService.completeShipment(me.customerOrderID, me.shipmentID,
				{ shipmentBox: $scope.shipmentBox, shipment: $scope.shipment },
				(res) => {
					if(res.data.success) {
						if(res.data.payload?.zipFile)
							downloadByteArray(res.data.payload.zipFile, res.data.payload.filename, "application/zip" );

						$scope.form.$setPristine();
						location.href = "/CustomerOrder";
					} else {
						$scope.completingShipment = false;
						console.error(res.data.errorMessage);
						alert(`Could not create label: ${res.data.errorMessage}`);
					}
				},
				(res) => {
					$scope.completingShipment = false;
					alert(`There was an error completing the shipment`);
					console.error(res);
				});
		}
	}
]);