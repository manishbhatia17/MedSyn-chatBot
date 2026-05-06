$app.controller("customerOrderListController", [
	"$scope", "$rootScope", "customerOrderService", "$uibModal", "$filter",
	function ($scope, $rootScope, customerOrderService, $uibModal, $filter) {
		
		$scope.canView        = $rootScope.checkClaims([SecurityKeys.CustomerOrderView]);
		$scope.canEdit        = $rootScope.checkClaims([SecurityKeys.CustomerOrderEdit]);
		$scope.canDoNotFill   = $rootScope.checkClaims([SecurityKeys.CustomerDoNotFillFlag]);
		$scope.canRevoke      = $rootScope.checkClaims([SecurityKeys.CustomerOrderRevokeOrder]);
		$scope.canFill        = $rootScope.checkClaims([SecurityKeys.CustomerOrderFulfillment]);
		$scope.canRescindFill = $rootScope.checkClaims([SecurityKeys.CustomerOrderFulfillmentRescind]);
		$scope.canShip        = $rootScope.checkClaims([SecurityKeys.CustomerOrderShippable]);
		$scope.canInvoice     = $rootScope.checkClaims([SecurityKeys.ToBeInvoiced]);
		$scope.canRescindShip = $rootScope.checkClaims([SecurityKeys.CustomerOrderShipRescind]);
		$scope.isManager      = $rootScope.checkClaims([
			SecurityKeys.CustomerOrderDomesticManagerApproval,
			SecurityKeys.CustomerOrderDomesticDistributorManagerApproval,
			SecurityKeys.CustomerOrderInternationalManagerApproval
		]);
		$scope.isVP = $rootScope.checkClaims([
			SecurityKeys.CustomerOrderDomesticVPApproval,
			SecurityKeys.CustomerOrderDomesticDistributorVPApproval,
			SecurityKeys.CustomerOrderInternationalVPApproval
		]);

		$scope.isAdmin = $rootScope.checkClaims([SecurityKeys.PriceAdjustmentEdit]);

		$scope.statusFilterValues = [];
		if($scope.canView || $scope.canEdit || $scope.isManager || $scope.isVP)
			$scope.statusFilterValues.push({ value: 1, text: "Waiting Submission" });
		if($scope.isManager)
			$scope.statusFilterValues.push({ value: 2, text: "Waiting Manager Approval" });
		if($scope.isVP)
			$scope.statusFilterValues.push({ value: 3, text: "Waiting VP Approval" });
		if($scope.canFill)
			$scope.statusFilterValues.push({ value: 4, text: "To be Filled" });
		if($scope.canRescindFill)
			$scope.statusFilterValues.push({ value: 11, text: "Filling" });
		if($scope.canShip)
			$scope.statusFilterValues.push({ value: 5, text: "To be Shipped" });
		if($scope.canInvoice){
			$scope.statusFilterValues.push({ value: 7, text: "To be Invoiced" });
		}
		if($scope.canView || $scope.canEdit || $scope.isManager || $scope.isVP)
			$scope.statusFilterValues.push({ value: 8, text: "Show my Orders" });
		if($scope.canDoNotFill)
			$scope.statusFilterValues.push({ value: 10, text: "Do Not Fill" });
		if ($scope.canInvoice || $scope.canView) 
			$scope.statusFilterValues.push({ value: 12, text: "Has Been Invoiced" });

		$scope.table = {
			idField: "customerOrderID",
			hideAdd: !$scope.canEdit,
			hideUpload: !$scope.isAdmin,
			hidePriceImport: true,
			showEdit: showEdit,
			showCheckbox: (tblScope) => !hideInvoiceSend(tblScope),
			checkboxField: (row) => row.customerOrderShipmentID,
			checkboxAction: { name: "Send", func: sendInvoice },
			columns: [{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" },
			{ "type": "html" }],
			dataColumns: [
				{ title: "Invoice Date", field: "invoiceDate", hide: hideInvoice, display: (val) => $filter("localTime")(val, "MM/dd/yyyy") },
				{ title: "Invoice #", field: "invoiceNumber", hide: hideInvoice, link: (val, row) => `/CustomerOrder/Invoice/${row.customerOrderShipmentID}` },
				{ title: "Order Date", field: "submitDate" },
				{ title: "Order #", field: "customerOrderCustomID" },
				{ title: "Cust ID", field: "customerCustomID" },
				{ title: "Cust Name", field: "customerName" },
				{ title: "Ship Loc", field: "locationName" },
				{ title: "Ship City", field: "locationCity", hide: hideInvoice },
				{ title: "Cust PO #", field: "poNumber", hide: hideCustomerPO },
				{ title: "Subtotal", field: "subtotal", display: (val) => $filter('currency')(val, "$"), hide: (x) => !hideInvoice(x) },
				{ title: "Total", field: "invoiceTotal", display: (val) => $filter('currency')(val, "$"), hide: hideInvoice },
				{ title: "Rep", field: "salesRep", hide: hideRep },
				{ title: "Status", field: "status", hide: hideStatus },
				{ title: "Being Filled By", field: "filledBy", hide: hideFill },
				{ title: "Order Status", field: "newFill", hide: hideFill}
			],
			actionColumns: [
				{ title: "Fill", func: fill, hide: hideFill },
				{ title: "Ship", func: ship, hide: hideShip },
				{ title: "Rescind", func: rescind, hide: hideRescind },
				{ title: "Rescind", func: rescindFilling, hide: hideRescindFilling },
				{ title: "Delete", func: deleteOrder, hide: hideDelete },
				{ title: "Upload", buttonText: "View File", func: viewUpload, hide: hideUpload, disabled: disableViewFile },
				{ title: "View PO", func: viewPO, hide: false },
				{ title: "View Order", func: viewOrder, hide: false }
			],
			additionalFilters: [
				{
					type: "dropdown",
					parameter: "status",
					values: $scope.statusFilterValues,
					onChange: changeFilter,
				},
				{
					type: "dropdown",
					parameter: "dateOption",
					values: [
						{ value: 1, text: "Today" },
						{ value: 2, text: "Yesterday" },
						{ value: 3, text: "Last 7 Days" },
						{ value: 4, text: "Last 30 Days" },
						{ value: 5, text: "This Month" },
					],
					disabled: hideDateOptions
				}
			],
			columnDefs: [{ "target": 8, type:'html'}]
		};

		SetPageRefresh();

		function RenderFillStatus(data, row) {
			return '<div style="background-color:red">data</div>';
		}

		


		function changeFilter(filterValue, tblScope) {
			
			tblScope.table.data = [];
			tblScope.table.allItems = [];
			if(filterValue === 7 || filterValue === 12)
				$scope.table.hideAdd = true;
			else
				$scope.table.hideAdd = !$scope.canEdit;
		}

		function fill(customerID) {
			location.href = `/CustomerOrder/Fill/${customerID}`;
		}

		function ship(customerID, row) {
			location.href = `/CustomerOrder/Ship/${customerID}?shipmentID=${row.customerOrderShipmentID}`;
		}

		function deleteOrder(customerOrderID, row) {
			if (!$scope.canEdit)
				return;
			customerOrderService.deleteCustomerOrder(customerOrderID, (res) => {
				if (res.data) {
					$scope.table.refresh();
				} else {
					console.error(res.data.errorMessage);
					alert("Could not delete customer order")
				}
			}, (err) => alert("Could not delete customer order"));
		}

		function rescind(customerOrderID, row, isFill) {
			if(!($scope.canRevoke || $scope.canRescindFill || $scope.canRescindShip))
				return;

			var isFillingParam = !!isFill;
			customerOrderService.rescindCustomerOrder(customerOrderID, row.customerOrderShipmentID, isFillingParam, (res) => {
				if(res.data) {
					$scope.table.refresh();
				} else {
					console.error(res.data.errorMessage);
					alert("Could not rescind customer order")
				}
			}, (err) => alert("Could not rescind customer order"));
		}

		function rescindFilling(customerOrderID, row) {
			rescind(customerOrderID, row, true)
		}

		function sendInvoice(shipmentIDs, callback) {
			if(!shipmentIDs?.length) {
				return;
			}

			var modal = $uibModal.open({
				templateUrl: "SendingInvoice.html",
				backdrop: "static",
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

			var taskFinished = function() {
				modal.dismiss();
				callback();
			};

			shipmentIDs = shipmentIDs.map((s) => parseInt(s));
			customerOrderService.sendInvoice(
				shipmentIDs,
				taskFinished,
				(res) => {
					console.log(res?.data?.errorMessage);
					alert("An error occurred while sending Invoice(s)");
					taskFinished();
				});
		}

		function viewUpload(orderId, customerOrder) {
			var a = document.createElement("a");
			a.href = customerOrder.attachmentURI;
			a.target = "_blank";

			document.body.appendChild(a);
			a.click();
			document.body.removeChild(a);
			URL.revokeObjectURL(a.href);
		}

		function showEdit(tblScope) {
			return ($scope.canEdit || $scope.canView) && ![5, 6, 7, 8, 12].includes(tblScope.additionalFilters.status);
		}

		function hideFill(tblScope) {
			return !$scope.canFill || ![4,11].includes(tblScope.additionalFilters.status);
		}

		function hideShip(tblScope) {
			return !$scope.canShip || tblScope.additionalFilters.status !== 5;
		}

		function hideUpload(tblScope) {
			return tblScope.additionalFilters.status !== 12 || !$scope.isAdmin;
		}

		function hideDelete(tblScope) {
			if (tblScope.additionalFilters.status === 1) {
				return !$scope.canEdit;
			}
			return true;
		}

		function hideRescind(tblScope) {
			if(tblScope.additionalFilters.status === 1 || tblScope.additionalFilters.status === 8) {
				return true;
			} else if(tblScope.additionalFilters.status === 2 || tblScope.additionalFilters.status === 3) {
				return !$scope.canRevoke;
		 	} else if(tblScope.additionalFilters.status === 4 || tblScope.additionalFilters.status === 10) {
				return !$scope.canRescindFill;
		 	} else if(tblScope.additionalFilters.status === 5) {
				return !$scope.canRescindShip;
			}
			return true;
		}

		function hideRescindFilling(tblScope) {
			return tblScope.additionalFilters.status !== 11;
		}

		function disableRescind(row, tblScope) {
			if (tblScope.additionalFilters.status === 4) {
				return true;
			} else if (tblScope.additionalFilters.status === 10) {
				return row.hasShipped;
			}
			else {
				return false;
			}
		}

		function disableViewFile(row, tblScope) {
			if(row.attachmentURI) {
				return false;
			}
			return true;
		}

		function hideRep(tblScope) {
			return tblScope.additionalFilters.status === 8;
		}

		function hideStatus(tblScope) {
			return tblScope.additionalFilters.status !== 8;
		}
		function hideInvoiceSend(tblScope) {
			return tblScope.additionalFilters.status !== 7;
		}

		function hideInvoice(tblScope) {
			return !(tblScope.additionalFilters.status === 7 || tblScope.additionalFilters.status === 12);
		}

		function hideDateOptions(tblScope){
			return tblScope.additionalFilters.status !== 12;
		}

		function hideCustomerPO(tblScope) {
			return !hideInvoice(tblScope);
		}

		function viewPO(row, tblScope) {
			//Added as per https://dev.azure.com/rkagathi/medforce/_workitems/edit/1/
			Object.assign(document.createElement('a'), {
				target: '_blank',
				href: tblScope.attachmentURI,
			}).click();
		}

		function viewOrder(row, tblScope) {
			debugger;
			Object.assign(document.createElement('a'), {
				href: "CustomerOrder/Details/" + tblScope.customerOrderID,
			}).click();
		}

		//refreshes the page every 15 minutes to ensure page is up to date
		function SetPageRefresh() {
			setTimeout(function () {
				console.log("refreshing");
				$scope.table.refresh();
				SetPageRefresh();
			}, 900000);
  
        }
	}
]);