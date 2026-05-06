$app.directive('listTable', function () {
	return {
		restrict: "E",
		templateUrl : "ListTableTemplate.html",
		scope: {
			table            : "=",
			service          : "@?",
			fetchFunc        : "@?",
			detailsRoute     : "@?",
			detailsFunc      : "=?",
			saveFunc         : "=?",
			addFunc          : "=?",
			backRoute        : "@?",
			refreshSearchFunc: "=?",
			searchCriteria: "=?",
			showAllPages: "=?",
		},
		controller: [
			"$scope", "$rootScope", "$injector", "$uibModal",
			function ($scope, $rootScope, $injector, $uibModal) {
				var service = $scope.service && $injector.get($scope.service);
				$scope.loading = false;
				$scope.pageSizes = [10, 25, 50, 100];
				$scope.searchCriteria = typeof $scope.searchCriteria !== 'undefined' ? $scope.searchCriteria : {
					sortColumn: $scope.table.dataColumns[0].field,
					sortAsc   : true,
					search    : "",
					pageSize  : 10,
					page      : 0
				};

				$scope.enableHtml = {value: $scope.table.allowHtml};
				$scope.selection = {allSelected: false, checkboxes: {}};
				$scope.isEditable = $scope.table.dataColumns.some(x => x.editable);
				$scope.table.showAllPages = $scope.showAllPages !== 'undefined' ? $scope.showAllPages : false;
				
				if (!$scope.saveFunc) {
					$scope.saveFunc = angular.noop;
				}

				if($scope.table.sorting === undefined)
					$scope.table.sorting = true;

				if($scope.table.additionalFilters) {
					var sessionFilters = sessionStorage.getItem(`${location.pathname}_additionalFilters`);
					if(sessionFilters)
						sessionFilters = JSON.parse(sessionFilters);
					$scope.additionalFilters = sessionFilters || {};

					$scope.table.additionalFilters.forEach(filter => {
						if(isFilterDisabled(filter))
							delete $scope.additionalFilters[filter.parameter];
					});
				}

				setDefaultFilters();

				$scope.onChange = function (row, col) {
					if (isFunction(col.onChange)) {
						col.onChange(row[col.field], row, $scope.table.data);
					}
				}

				$scope.isColumnEditable = function(row, col) {
					return isFunction(col.editable) ? col.editable(row[col.field], row) : col.editable;
				}

				$scope.isColumnRequired = function(row, col) {
					return isFunction(col.required) ? col.required(row[col.field], row) : col.required;
				}

				$scope.isActionDisabled = function(row, col) {
					return isFunction(col.disabled) ? col.disabled(row, $scope) : col.disabled;
				}

				function isFilterDisabled(filter) {
					return isFunction(filter.disabled) ? filter.disabled($scope) : filter.disabled;
				}
				$scope.isFilterDisabled = isFilterDisabled;

				$scope.columnFilterFn = function(col) {
					return isFunction(col.hide) ? !col.hide($scope) : !col.hide;
				}

				$scope.showEdit = function() {
					return isFunction($scope.table.showEdit) ? $scope.table.showEdit($scope) : $scope.table.showEdit;
				}

				$scope.showCheckbox = function() {
					return isFunction($scope.table.showCheckbox) ? $scope.table.showCheckbox($scope) : $scope.table.showCheckbox;
				}

				$scope.checkboxField = function(row) {
					return isFunction($scope.table.checkboxField) ? $scope.table.checkboxField(row) : $scope.table.checkboxField;
				}

				$scope.hasChecked = function() {
					var checkboxes = Array.from(document.querySelectorAll("input[type='checkbox'].table-selection"));
					return checkboxes.findIndex((c) => c.checked) !== -1;
				}

				$scope.selectAll = function() {
					var checkboxes = Array.from(document.querySelectorAll("input[type='checkbox'].table-selection"));
					checkboxes.forEach(function (c) {
						c.checked = $scope.selection.allSelected;
					});
				};

				$scope.getLink = function(row, col) {
					return isFunction(col.link) ? col.link(row[col.field], row) : col.link;
				}

				$scope.sort = function (column) {
					if ($scope.searchCriteria.sortColumn !== column) {
						$scope.searchCriteria.sortAsc = true;
					} else {
						$scope.searchCriteria.sortAsc = !$scope.searchCriteria.sortAsc;
					}

					$scope.searchCriteria.sortColumn = column;
					$scope.refreshSearch();
				};

				$scope.goToPage = function (page) {
					if (page < 0 || page >= $scope.table.totalPages) {
						return;
					}
					$scope.searchCriteria.page = page;
					$scope.refreshSearch();
				};

				$scope.filterChanged = function(filter)
				{
					if(isFunction(filter.onChange))
						filter.onChange($scope.additionalFilters[filter.parameter], $scope);

					$scope.table.additionalFilters.forEach(filter => {
						if($scope.isFilterDisabled(filter))
							delete $scope.additionalFilters[filter.parameter];
					});
					setDefaultFilters();
					$scope.searchCriteria.page = 0;
					$scope.refreshSearch();
				}

				$scope.refreshSearch = function () {
					if($scope.listForm?.$dirty && $scope.isEditable) {
						var save = window.confirm("You have unsaved changes. Click OK to save, or Cancel to discard.")
						if(save) {
							$scope.callSaveFunc();
							return;
						}
					}

					if (service && $scope.fetchFunc) {						
						var func = service[$scope.fetchFunc];
						
						if($scope.additionalFilters) {
							sessionStorage.setItem(`${location.pathname}_additionalFilters`, JSON.stringify($scope.additionalFilters));
							func($scope.searchCriteria, $scope.additionalFilters, searchCB);
							$scope.loading = true;
						} else {
							func($scope.searchCriteria, searchCB);
							$scope.loading = true;
						}
					} else {
						localSearch();
					}
				};
				// allow for outside controllers to refresh the search
				$scope.table.refresh = $scope.refreshSearch;

				$scope.goToDetails = function (id, row) {
					if($scope.detailsRoute) {
						location.href = `${$scope.detailsRoute}${id}`;
					} else {
						$scope.detailsFunc(id, row);
					}
				};

				$scope.goBack = function() {
					if($scope.listForm.$dirty) {
						var save = window.confirm("You have unsaved changes. Click OK to save, or Cancel to discard.")
						if(save) {
							$scope.callSaveFunc();
							return;
						} else {
							$scope.listForm.$setPristine();
						}
					}
					location.href = $scope.backRoute;
				}

				$scope.callSaveFunc = function() {
					if($scope.listForm.$invalid) {
						$rootScope.showInvalidFields($scope.listForm);
						return;
					}

					$scope.isSaving = true;
					$scope.saveFunc($scope.table.data, $scope.listForm, (success) => {
						$scope.isSaving = false;
						$scope.saveSuccess = success;
						$scope.refreshSearch()
					});
				}

				$scope.callAddFunc = function() {
					if(!$scope.addFunc) {
						$scope.goToDetails(0);
					} else {
						$scope.addFunc($scope.table.data, $scope.listForm);
					}
				}

				$scope.callUploadOrderFunc = function () {
					var modal = $uibModal.open({
						templateUrl: "UploadOrderModal.html",
						backdrop: "static",
						keyboard: false,
						resolve: {
							data: function () {
								return {
									
								}
							}, service: $scope.service
						},
						controller: [
							"$scope", "$uibModalInstance", "data", "service",


							function ($scope, $uibModalInstance, data, service) {
								Object.assign($scope, data);
								const fileReader = new FileReader();
								let fileString = "";
								let fileName = "";
								
								fileReader.addEventListener("load", function () {
									fileString = fileReader.result;
								});

								$scope.close = function () {
									$uibModalInstance.dismiss();
								}

								$scope.uploadOrder = function () {
									
									if (fileString.length > 0) {
										$scope.modalLoading = true;
										service.uploadOrder({ "File": fileString, "FileName": "test" }, function (response) {
											console.log(response);
											if (response.data.customerShippingInfoID <= 0)
												alert("Order created with issues: Shipping Info not found, review shipping information and update in order");
											window.location.href = `CustomerOrder/Details/${response.data.customerOrderID}`;
											$scope.modalLoading = false;
										
										}, function (err) { alert(err.data); $scope.modalLoading = false; });
									}
								}

								$scope.onFileChange = function (input) {
									fileReader.readAsDataURL(input.files[0]);
									fileName = input.files[0].name;
								}
							}
						]
					});
				}

				$scope.callImportPriceFunc = function () {
					var modal = $uibModal.open({
						templateUrl: "ImportPriceAdjustments.html",
						backdrop: "static",
						keyboard: false,
						resolve: {
							data: function () {
								return {

								}
							},
							service: $scope.service,
							$root: $scope
						},
						controller: [
							"$scope", "$uibModalInstance", "data", "service", "$root",


							function ($scope, $uibModalInstance, data, service, $root) {
								Object.assign($scope, data);
								const fileReader = new FileReader();
								let fileString = "";
								let fileName = "";

								fileReader.addEventListener("load", function () {
									fileString = fileReader.result;
								});

								$scope.close = function () {
									$uibModalInstance.dismiss();
								}

								$scope.uploadPrices = function () {

									if (fileString.length > 0) {
										$scope.modalLoading = true;
										service.uploadPrices({ "File": fileString, "FileName": "test" }, function (response) {
											$root.refreshSearch();
											alert("Prices imported successfully");											
											$scope.modalLoading = false;
											$uibModalInstance.dismiss();

										}, function (err) { alert(err.data); $scope.modalLoading = false; });
									}
								}

								$scope.onFileChange = function (input) {
									fileReader.readAsDataURL(input.files[0]);
									fileName = input.files[0].name;
								}
							}
						]
					});
				}

				$scope.callCheckboxFunc = function() {
					if(!$scope.table?.checkboxAction?.func) {
						return;
					}
					var checkboxes = Array.from(document.querySelectorAll("input[type='checkbox'].table-selection"));
					var values = checkboxes
						.filter((c) => c.checked)
						.map((c) => c.getAttribute("data-field-value"));
					$scope.table.checkboxAction.func(values, $scope.refreshSearch);
				}

				// Make the initial fetch call
				$scope.refreshSearch();

				function searchCB(response) {
					$scope.listForm.$setPristine();
					var data = response.data;

					var { results, ...meta } = data;
					Object.assign($scope.table, {
						...meta,
						data: data.results,
						pages: Array(data.totalPages)
					});

					$scope.loading = false;
				}

				function localSearch() {
					$scope.table.data = $scope.table.data?.sort((a, b) => {
						var aVal = a[$scope.searchCriteria.sortColumn];
						var bVal = b[$scope.searchCriteria.sortColumn];

						var ret = 0;
						if(aVal === null || aVal === undefined) {
							ret = 1
						}
						else if(bVal === null || bVal === undefined) {
							ret = -1
						}
						else if(typeof aVal === "number") {
							ret = aVal - bVal;
						} else {
							ret = aVal.localeCompare(bVal);
						}

						return $scope.searchCriteria.sortAsc ? ret : -ret;
					});
				}

				function setDefaultFilters() {
					$scope.table.additionalFilters?.forEach((filter) => {
						if($scope.additionalFilters[filter.parameter] !== undefined || isFilterDisabled(filter))
							return;
	
						switch(filter.type) {
							case "dropdown":
								$scope.additionalFilters[filter.parameter] = filter.values.find(x => x.selected)?.value || filter.values[0].value;
								break;
							case "checkbox":
								$scope.additionalFilters[filter.parameter] = false;
								break;
							case "date":
								{
									console.log(filter.value);
									$scope.additionalFilters[filter.parameter] = filter.value;
									break;
								}
							case "static":
								$scope.additionalFilters[filter.parameter] = filter.value;
						}
					});
				}
			}
		],
	};


});