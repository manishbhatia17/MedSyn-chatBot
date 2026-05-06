$app.controller("customerDetailsController", [
	"$scope", "$rootScope", "$uibModal", "customerService",
	function ($scope, $rootScope, $uibModal, customerService) {
		var me = this;
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.CustomerEdit]);
		$scope.AddEditShippingText = "Add Info";
		$scope.saveError = "";

		$scope.table = {
			idField     : "customerShippingInfoID",
			showEdit    : $scope.canEdit,
			hideSearch  : true,
			hideAdd     : true,
			noPagination: true,
			dataColumns: [
				{ title: "Name", field: "displayName" },
				{ title: "Address1", field: "address" },
				{ title: "Address2", field: "address2" },
				{ title: "City", field: "city" },
				{ title: "St", field: "stateCodeID", display: (value) => $scope.stateCodes.find(x => x.value === value)?.text },
				{ title: "Zip", field: "zipCode" },
				{ title: "Country", field: "countryCodeID", display: (value) => $scope.countryCodes.find(x => x.value === value)?.text },
				{ title: "Rep", field: "repUserID", display: (value) => $scope.repUserIDs.find(x => x.value === value)?.text },
				{ title: "Ship Co # 1", field: "shipCompany1CodeID", display: (value) => $scope.shipCompanyCodes.find(x => x.value === value)?.text },
				{ title: "Account Info", field: "shipCompany1AccountNumber" },
				{ title: "Ship Co # 2", field: "shipCompany2CodeID", display: (value) => $scope.shipCompanyCodes.find(x => x.value === value)?.text },
				{ title: "Account Info", field: "shipCompany2AccountNumber" },
			],
			actionColumns: [
				{ title: "Order", func: order }
			],
		};

		function order(shipmentId) {
			location.href = `/customerOrder/details/0?customerID=${$scope.customer.customerID}&customerShippingID=${shipmentId}`;
		}

		$scope.viewOnly = function () {
			return !$scope.canEdit;
		}

		$scope.save = function () {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			customerService.saveCustomer($scope.customer, (res) => {
				if(res.data.success) {
					$scope.form.$setPristine();
					location.href = "/customer";
				} else {
					if(res.data.errorMessage === "DUP_ID") {
						$scope.form.customerCustomID.$setValidity('unique', false);
					} else {
						alert("Error saving Customer. " + err.data);
						console.error(res.data.errorMessage, res.data.fullError);
					}
				}
			}, (err) => { console.log(err); alert(`failed to save customer data: ${err.data}`);});
		};


		$scope.clearInvalid = function() {
			$scope.form.customerCustomID.$setValidity('unique', true);
		}

		$scope.init = function (customerID) {
			me.customerID = customerID;
			customerService.getCustomerDetails(customerID,
				(res) => {
					$scope.customer                    = res.data.customer;
					$scope.stateCodes                  = res.data.stateCodes;
					$scope.countryCodes                = res.data.countryCodes;
					$scope.glSalesNumberCodes          = res.data.glSalesNumberCodes;
					$scope.glShippingChargeNumberCodes = res.data.glShippingChargeNumberCodes;
					$scope.glReceivableNumberCodes     = res.data.glReceivableNumberCodes;
					$scope.practiceTypeCodes           = res.data.practiceTypeCodes;
					$scope.salesTaxCodes               = res.data.salesTaxCodes;
					$scope.paymentTypes                = res.data.paymentTypes;
					$scope.customerStatusCodes         = res.data.customerStatusCodes;
					$scope.repUserIDs                  = res.data.repUserIDs;
					$scope.shipCompanyCodes            = res.data.shipCompanyCodes;

					$scope.customer.shippingInfo.map((item) => item.displayName = (item.isDisabled ? 'Disabled - ' : '') + item.name);
					$scope.table.data = $scope.customer.shippingInfo;
					$scope.customer.paymentTypeCodeID = $scope.customer.paymentTypeCodeID ?? $scope.paymentTypes
						.find((pt) => pt.altID?.toLowerCase() === "check")?.value;
				}
			)
		}
		
		$scope.addShippingInfo = function () {

			if (me.customerID === 0) {
				if ($scope.form.$invalid) {
					$rootScope.showInvalidFields($scope.form);
					return;
				}

				customerService.saveCustomer($scope.customer, (res) => {
					if (res.data.errorMessage === "DUP_ID") {
						$scope.form.customerCustomID.$setValidity('unique', false);
					}
					else if (typeof res.data  === 'number') {
						console.log(res.data);
						$scope.init(res.data);
						$scope.editShippingInfo(0);
					}
					else {
						alert("Error saving Customer. " + err.data);
						console.error(res.data.errorMessage, res.data.fullError);
					}
				}, (err) => { console.log(err); alert(`failed to save customer data: ${err.data}`); });
				
			}
			else {
				$scope.editShippingInfo(0);
			}

			
		}

		$scope.editShippingInfo = function (id) {
			if (me.customerID === 0) {
				alert('Please save the customer before adding shipping info.');
				return;
			}
			
			var shippingInfo = {};
			if (!id) {
				$scope.AddEditShippingText = "Add Info";
				shippingInfo.customerID             = me.customerID;
				shippingInfo.customerShippingInfoID = -$scope.customer.shippingInfo.length - 1;
				shippingInfo.countryCodeID          = $scope.countryCodes.find(x => x.altID === "USA").value;
			} else {
				$scope.AddEditShippingText = "Update Info";
				var existingInfo = $scope.customer.shippingInfo.find(x => x.customerShippingInfoID === id);
				Object.assign(shippingInfo, existingInfo);
			}

			var modal = $uibModal.open({
				templateUrl: "ShippingInfoModal.html",
				backdrop: "static",
				keyboard: false,
				resolve: {
					data: function() {
						return {
							shippingInfo    : shippingInfo,
							stateCodes      : $scope.stateCodes,
							countryCodes    : $scope.countryCodes,
							repUserIDs      : $scope.repUserIDs,
							shipCompanyCodes: $scope.shipCompanyCodes,
							addEditShippingText: $scope.AddEditShippingText,
							customer: $scope.customer,
							saveError: $scope.saveError
						}
					}
				},
				controller: [
					"$scope", "$uibModalInstance", "data",
					function ($scope, $uibModalInstance, data) {
						Object.assign($scope, data);
						$scope.AddEditShippingText = data.addEditShippingText;
						$scope.close = function() {
							$uibModalInstance.dismiss();
						}

						$scope.add = function() {
							if($scope.form.$invalid) {
								$rootScope.showInvalidFields($scope.form);
								return;
							}

							$uibModalInstance.close($scope.shippingInfo);
						}

						$scope.delete = function () {
							//todo: alert

							$uibModalInstance.close(data.shippingInfo.customerShippingInfoID);
                        }
					}
				]
			});

			modal.result.then(
				function (data) {
					if (!isNaN(parseInt(data))) {

						//$scope.form.$setDirty();
						customerService.deleteCustomerShippingInfo($scope.customer.customerID, data, (results) => {

							while ($scope.customer.shippingInfo.length > 0) {
								$scope.customer.shippingInfo.pop();
							}

							for (var i = 0; i < results.data.length; i++) {
								results.data[i].displayName = (results.data[i].isDisabled ? 'Disabled - ' : '') + results.data[i].name;
								$scope.customer.shippingInfo.push(results.data[i]);
							}

							//$scope.form.$setDirty();
							document.getElementById("save-success").style.display = "block";

							setTimeout(() => { document.getElementById("save-success").style.display  = "none"; }, 5000);
						}, ((err) => {
							$scope.saveError = err.data;
							document.getElementById("save-fail").style.display = "block";

						}));
					}
					else {
						var existing = $scope.customer.shippingInfo.find(x =>
							x.customerShippingInfoID === data.customerShippingInfoID
						);

						if (existing) {
							Object.assign(existing, data);
						} else {
							data.displayName = (data.isDisabled ? 'Disabled - ' : '') + data.name;
							$scope.customer.shippingInfo.push(data);
						}

						customerService.saveCustomerShippingInfo(data, ((results) => {

							while ($scope.customer.shippingInfo.length > 0) {
								$scope.customer.shippingInfo.pop();
							}
						
							for (var i = 0; i < results.data.length; i++) {
								results.data[i].displayName = (results.data[i].isDisabled ? 'Disabled - ' : '') + results.data[i].name;
								$scope.customer.shippingInfo.push(results.data[i]);
                            }

							//$scope.form.$setDirty();
							document.getElementById("save-success").style.display = "block";

							setTimeout(() => { document.getElementById("save-success").style.display = "none"; }, 5000);

						}), ((err) => {
							$scope.saveError = err.data;
							document.getElementById("save-fail").style.display = "block";

							//setTimeout(() => { document.getElementById("save-fail").display = "none"; }, 5000);

						}));
					}
				},
				function (err) {
					$scope.saveError = err;
				}
			);
		}
	}
]);