$app.controller("customerOrderDetailsController", [
	"$scope", "$rootScope", "customerOrderService", "customerService", "$uibModal",
	function ($scope, $rootScope, customerOrderService, customerService, $uibModal) {
		var me = this;

		$scope.viewOnly = function () {
			return !($scope.canEdit || $scope.isManager || $scope.isVP);
		}

		$scope.isDoNotFillEnabled = function () {
			return $scope.canDoNotFill && $scope.customerOrder && ($scope.customerOrder.vpApprovedBy || $scope.customerOrder.vpApprovedOn);
		}

		$scope.save = function (submit) {

			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}


			var invalidProductId = $scope.customerOrder.products.some(function (p, i) {
				if (p.productID <= 0) return false;
				return true;
			});

		
			$scope.isSaving = true;
			$scope.formatPrices();
			customerOrderService.saveCustomerOrder($scope.customerOrder, submit, () => {
				$scope.form.$setPristine();
				var co = $scope.customerOrder;
				if(co.submitDate && (!co.mgApprovedBy || !co.vpApprovedBy)) {
					$scope.isSaving = false;
					return;
				}

				if(submit)
					location.href = "/customerOrder";
				else
					alert(`Customer Order saved successfully`)

				$scope.isSaving = false;
			}, (ex) => {
				console.log(ex);
				alert(ex.data);
				$scope.isSaving = false;
			});
		};

		$scope.init = function (customerOrderID) {
			
			$scope.canEdit = $rootScope.checkClaims([SecurityKeys.CustomerOrderEdit]);
			$scope.canDoNotFill = $rootScope.checkClaims([SecurityKeys.CustomerDoNotFillFlag]);
			$scope.canExport = $rootScope.checkClaims([SecurityKeys.CustomerOrderExport]);
			$scope.customerOrderID = customerOrderID;
			var urlParams  = new URLSearchParams(location.search);
			var customerID = urlParams.get("customerID");
			

			customerOrderService.getCustomerOrderDetails(customerOrderID, { customerID: customerID },
				(res) => {
					$scope.customerOrder = res.data.customerOrder;
					$scope.customers = res.data.customers;
					$scope.products = res.data.products;
					$scope.umCodes = res.data.umCodes;
					$scope.customerTaxes = res.data.customerTaxes;
					$scope.customerCreditCardFee = res.data.creditCardFee;
					if (urlParams.get("customerShippingID") !== null) {
						$scope.customerOrder.customerShippingInfoID = parseInt(urlParams.get("customerShippingID"));
					}
					$scope.changeShipCompany();

					if ($scope.customerOrder.customerOrderID > 0) {
						if ($scope.customerOrder.isDomestic) {
							$scope.isManager = $rootScope.checkClaims([SecurityKeys.CustomerOrderDomesticManagerApproval]);
							$scope.isVP = $rootScope.checkClaims([SecurityKeys.CustomerOrderDomesticVPApproval]);
						}
						if ($scope.customerOrder.isDomesticDistribution) {
							$scope.isManager = $rootScope.checkClaims([SecurityKeys.CustomerOrderDomesticDistributorManagerApproval]);
							$scope.isVP = $rootScope.checkClaims([SecurityKeys.CustomerOrderDomesticDistributorVPApproval]);
						}
						if ($scope.customerOrder.isInternational) {
							$scope.isManager = $rootScope.checkClaims([SecurityKeys.CustomerOrderInternationalManagerApproval]);
							$scope.isVP = $rootScope.checkClaims([SecurityKeys.CustomerOrderInternationalVPApproval]);
						}
					}

					me.shipCompanyCodes = res.data.shipCompanyCodes;
					me.fedExCodeID = me.shipCompanyCodes.find(x => x.altID === "FedEx").value;
					me.fedExMedGynCodeID = me.shipCompanyCodes.find(x => x.altID === "FedEx - MedGyn").value;
					me.upsCodeID = me.shipCompanyCodes.find(x => x.altID === "UPS").value;
					me.upsMedGynCodeID = me.shipCompanyCodes.find(x => x.altID === "UPS - MedGyn").value;

					me.fedExShipMethodCodes = res.data.fedExShipMethodCodes;
					me.upsShipMethodCodes = res.data.upsShipMethodCodes;
					me.otherShipMethodCodes = res.data.otherShipMethodCodes;

					if ($scope.customerOrder.customerID) {
						$scope.changeCustomer();
					}

					if (!$scope.customerOrder.products.length) {
						$scope.addProduct();
					}
					else {
						$scope.formatPrices();
					}
					$scope.customerProducts = $scope.products;

					//if ($scope.customerOrder.customerID){
					//	$('#customer-select').prop('disabled', true);
					//}
					//else {
					//	$('#customer-select').prop('disabled', false);
					//}

				}
			)

			
		}

		$("#co-file-upload").on("change", () => {
			var file = event.target.files[0];
			if(!file) {
				return;
			}
			$scope.customerOrder.attachmentFileName = file.name;
			var reader = new FileReader();

			reader.onload = function (e) {
				$scope.coFile = e.target.result;
				$scope.customerOrder.attachmentURI = e.target.result;
				$scope.isAttachmentChanged = true;
				$scope.form.$setDirty();
				$scope.$apply();
			}

			reader.readAsDataURL(file);
		});

		$scope.formatPrices = function(){
			$scope.customerOrder.products.forEach(function(product) {
				if(product.price || product.price === 0)
					product.price = parseFloat(product.price).toFixed(4);
			});
		};

		$scope.changeCustomer = function () {
			if (!$scope.customerOrder.shippingCustomerName) {
				$scope.customerOrder.shippingCustomerName = $scope.customers.find(e => e.value ===
					$scope.customerOrder.customerID).text.split(/ (.+)/)[1];
			}
			customerService.getCustomerDetails($scope.customerOrder.customerID,
				(res) => {
					var customer        = res.data.customer;
					customer.shippingInfo = customer.shippingInfo.filter(function (shippingInfo) {
						return !shippingInfo.isDisabled;
					});

					var types = []
					if(customer.customerType === 1) customer._type = "Domestic";
					if(customer.customerType === 2) customer._type = "Domestic Afaxys";
					if(customer.customerType === 3) customer._type = "Domestic Distributor";
					if(customer.customerType === 4) customer._type = "International";

					var terms = {
						1: "COD",
						2: "Prepay",
						3: `Net Due in ${customer.paymentTermsNetDueDays} days`,
					}

					customer._terms        = terms[customer.paymentTermsType];

					$scope.customer = customer;

					$scope.practiceTypesDict = res.data.practiceTypeCodes.reduce((agg, cur) => {
						return { ...agg, [cur.value]: cur.text };
					}, {});

					$scope.shippingInfoDict = customer.shippingInfo.reduce((agg, cur) => {
						return { ...agg, [cur.customerShippingInfoID]: cur };
					}, {});

					$scope.repDict = res.data.repUserIDs.reduce((agg, cur) => {
						return { ...agg, [cur.value]: cur.text };
					}, {});

					if($scope.customerOrder.customerShippingInfoID) {
						$scope.changeShipLocation();
					}

					$scope.customerProducts = $scope.products;
					if(customer.customerType !== 4) {
						$scope.customerProducts = $scope.products.filter((p) => !p.data.isInternationalOnly);
					}

					$scope.customerOrder.products.forEach((p, i) => {
						$scope.changeProduct(i);
					});
				}
			)
		}

		$scope.changeShipLocation = function () {
			var shipInfo = $scope.shippingInfoDict[$scope.customerOrder.customerShippingInfoID];
			$scope.shipCompanies = [];
			$scope.shippingAccounts = {};

			// if the user changes the customer, null out exisint shipping info
			if(!shipInfo){
				$scope.customerOrder.customerShippingInfoID = null;
				$scope.customerOrder.shipCompanyType        = null;
				$scope.customerOrder.shipChoiceCodeID       = null;
				$scope.shipChoiceCodes                      = null;
				return;
			}

			if(shipInfo.shipCompany1CodeID){
				var shipCompany = me.shipCompanyCodes.find(x => x.value === shipInfo.shipCompany1CodeID);
				$scope.shipCompanies.push(shipCompany)
				$scope.shippingAccounts[shipCompany.value] = shipInfo.shipCompany1AccountNumber;
			}
			if(shipInfo.shipCompany2CodeID){
				var shipCompany = me.shipCompanyCodes.find(x => x.value === shipInfo.shipCompany2CodeID);
				$scope.shipCompanies.push(shipCompany)
				$scope.shippingAccounts[shipCompany.value] = shipInfo.shipCompany2AccountNumber;
			}

			if($scope.customerOrder.shipCompanyType) {
				// null out ship company if it's not a valid option anymore
				if(!$scope.shipCompanies.some(x => x.value === $scope.customerOrder.shipCompanyType)){
					$scope.customerOrder.shipCompanyType        = null;
					$scope.customerOrder.shipChoiceCodeID       = null;
					$scope.shipChoiceCodes                      = null;
					return;
				}

				$scope.changeShipCompany();
			}
		}

		$scope.changeShipCompany = function(){
			if($scope.customerOrder.shipCompanyType === me.fedExCodeID || $scope.customerOrder.shipCompanyType === me.fedExMedGynCodeID)
				$scope.shipChoiceCodes = me.fedExShipMethodCodes
			else if($scope.customerOrder.shipCompanyType === me.upsCodeID || $scope.customerOrder.shipCompanyType === me.upsMedGynCodeID)
				$scope.shipChoiceCodes = me.upsShipMethodCodes
			else if(!$scope.customerOrder.shipCompanyType)
				$scope.shipChoiceCodes = [];
			else
				$scope.shipChoiceCodes = me.otherShipMethodCodes
		}

		$scope.changeProduct = function (idx) {
           
			// Get the selected product ID
			var selectedId = $scope.customerOrder.products[idx].productID;

			// If nothing is selected, just return
			if (!selectedId && selectedId <= 0) return;

			// Check for duplicate productID in the list (excluding current index and deleted rows)
			var duplicate = $scope.customerOrder.products.some(function (p, i) {
				if (i === idx) return false;
				if (p.markedForDelete) return false;
				if (!p.productID && p.productID !== 0) return false;
				return p.productID == selectedId;
			});

			//todo: if original id was - then do this otherwise warn the user to combine so items dont get deleted
			if (duplicate) {
				var product = $scope.products.find((p) => {
					return p.value == selectedId
				});

				console.log(product)
				alert(`Product ${product ? product.text : ''} has already been added to this order. Please update the quantity on the existing line item.`);
				// Only delete if the original product is new (negative customerOrderProductID)
				if ($scope.customerOrder.products[idx].customerOrderProductID < 0) {
					$scope.deleteProduct(idx);
					$scope.addProduct();
				} else {

					// RReset the dropdown to the original value
					$scope.customerOrder.products[idx].productID = $scope.customerOrder.products[idx].originalProductID;
				}
				return;
			}

			// If not a duplicate, update originalProductID to the new selection
			$scope.customerOrder.products[idx].originalProductID = selectedId;

			customerOrderService.getCustomerOrderHistoryForProduct($scope.customerOrder.customerID, $scope.customerOrder.products[idx].productID,
				(res) => {
					var product = $scope.customerOrder.products[idx]
					product.meta = res.data;
					product.unitOfMeasureCodeID = res.data.umCodeID;
					product.poExpectedDates = res.data.poExpectedDates;
					if($scope.customer){
						if($scope.customer.customerType === 1)
							product.meta.listPrice = res.data.priceDomesticList
						else if($scope.customer.customerType === 2)
							product.meta.listPrice = res.data.priceDomesticAfaxys
						else if($scope.customer.customerType === 3)
							product.meta.listPrice = res.data.priceDomesticDistribution
						else if($scope.customer.customerType === 4)
							product.meta.listPrice = res.data.priceInternationalDistribution
					}
				}
			)
		}

		$scope.addProduct = function() {
			$scope.customerOrder.products.push({
				customerOrderProductID : -$scope.customerOrder.products.length - 1
			});
		}

		$scope.deleteProduct = function(idx) {
			var product = $scope.customerOrder.products[idx];

			if(product.customerOrderProductID > 0) {
				product.markedForDelete = true;
				$scope.form.$setDirty();
			} else {
				$scope.customerOrder.products.splice(idx, 1);
			}
		}

		$scope.approve = function(isVPApproval) {
			customerOrderService.approveCustomerOrder($scope.customerOrderID, isVPApproval, (res) => {
				if(res.data.success) {
					$scope.form.$setPristine();
					location.href = "/customerOrder";
				} else {
					console.error(res.data.errorMessage);
					alert("Could not approve customer order")
				}
			}, (err) => alert("Could not approve customer order"));
		}

		$scope.totalProducts = function() {
			return $scope.customerOrder?.products.length;
		}

		$scope.totalItems = function() {
			return $scope.customerOrder?.products.reduce((a, b) => a + (b.quantity || 0), 0)
		}

		$scope.totalPrice = function() {
			return $scope.customerOrder?.products.reduce((a, b) => a + ((b.quantity * b.price) || 0), 0)
		}

		$scope.totalShippingCharge = function() {
			if(!$scope.customerOrder) return 0;

			return $scope.customerOrder.shippingCharge +
				$scope.customerOrder.handlingCharge +
				$scope.customerOrder.insuranceCharge;
		}

		$scope.tax = function () {
			if ($scope.customerOrder?.customerID) {
				if ($scope.customerTaxes[$scope.customerOrder.customerID]) {
					var tax = $scope.customerTaxes[$scope.customerOrder.customerID];
					return $scope.totalPrice() * (tax / 100);
				}
				else
					return 0;
			}
		}

		$scope.creditCardFee = function () {
			if ($scope.customerOrder?.customerID) {
				if ($scope.customerCreditCardFee[$scope.customerOrder.customerID]) {
					var fee = $scope.customerCreditCardFee[$scope.customerOrder.customerID];
					return ($scope.totalPrice() + $scope.totalShippingCharge() + $scope.tax()) * (fee / 100);
				}
				else
					return 0;
			}
		}

		$scope.export = function() {
			$uibModal.open({
				templateUrl: "ExportPopup.html",
				backdrop: "static",
				keyboard: false,
				resolve: {
					data: function() {
						return {
							documentTitle: "Order Confirmation",
							customerOrderID: $scope.customerOrder.customerOrderID
						}
					}
				},
				controller: [
					"$scope", "$uibModalInstance", "data",
					function($scope, $uibModalInstance, data) {
						Object.assign($scope, data);

						$scope.close = function() {
							$uibModalInstance.dismiss();
						}

						$scope.export = function() {
							if($scope.form.$invalid) {
								$rootScope.showInvalidFields($scope.form);
								return;
							}

							window.open(`/CustomerOrder/Report/${$scope.customerOrderID}?documentTitle=${$scope.documentTitle}`, "_blank")
							$uibModalInstance.close();
						}
					}
				]
			}).result.then(angular.noop, angular.noop);
		}

		$scope.doNotFillClick = function () {
			$uibModal.open({
				templateUrl: "DoNotFillPopup.html",
				backdrop: "static",
				keyboard: false,
				resolve: {
					data: function () {
						return {
							doNotFill: $scope.customerOrder.isDoNotFill,
							doNotFillReason: $scope.customerOrder.doNotFillReason,
							customerOrderID: $scope.customerOrder.customerOrderID
						}
					}
				},
				controller: [
					"$scope", "$uibModalInstance", "data",
					function ($scope, $uibModalInstance, data) {
						Object.assign($scope, data);

						$scope.close = function () {
							$uibModalInstance.close($scope.doNotFill);
						}

						$scope.saveFill = function () {
							var doNotFillDTO = {
								doNotFill: $scope.doNotFill,
								doNotFillReason: $scope.doNotFillReason
							}

							customerOrderService.doNotFill($scope.customerOrderID, doNotFillDTO, () => { location.href = "/customerOrder"; });
						}
					}
				]
			}).result.then(function (data) { console.log(data); $scope.customerOrder.isDoNotFill = !data}, angular.noop);
		}
		$('#customer-select').select2({
			width: 'resolve' 
		});
	}
]);