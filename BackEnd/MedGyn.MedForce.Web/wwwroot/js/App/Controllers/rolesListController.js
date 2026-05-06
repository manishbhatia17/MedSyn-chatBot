$app.controller("rolesListController", [
	"$scope", "$rootScope", "$uibModal", "roleService",
	function ($scope, $rootScope, $uibModal, roleService) {
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.RoleEdit]);
		$scope.table = {
			idField: "roleId",
			hideAdd: !$scope.canEdit,
			hideSave: !$scope.canEdit,
			dataColumns: [
				{ title: "Role Name", field: "roleName", editable: $scope.canEdit, maxlength: 100, onChange: (val, row, list) => $scope.nameChange(val, row, list) },
				{ title: "Role ID", field: "roleId", editable: false},
				{ title: "Number of Users", field: "userCount", editable: false},
				{ title: "Number of Keys", field: "keyCount", editable: false},
				{ title: "Archive", field: "isArchived", type: "checkbox", editable: (val, row) => $scope.canEdit && row.userCount == 0},
			],
			actionColumns: [
				{ title: "Permissions", buttonText: "Give", func: (id, row) => $scope.editPermissions(id, row), disabled: (row) => !$scope.canEdit || row.roleId <= 0 || row.isArchived }
			]
		};

		$scope.nameChange = function(val, row, list) {
			let nameCount = 0, id = `roleName_${row.roleId}`;
			list.forEach(function (role) {
				if(role.roleName === val)
					nameCount++;
			});
			if(nameCount > 1) {
				var el = angular.element(`#${id}`);
				angular.element(`#${id}_errors`).scope().uniqueMsg = "Role Name must be unique.";
				el.closest('form').scope().listForm[id].$setValidity('unique', false);
			}
			else {
				var el = angular.element(`#${id}`);
				el.closest('form').scope().listForm[id].$setValidity('unique', true);
			}
		};

		$scope.editPermissions = function(id, role) {
			roleService.fetchKeysForRole(id, (res) => {
				var isAllSelected = true;
				res.data.forEach(function (key) {
					if(!key.isSelected) {
						isAllSelected = false;
					}
				});
				$uibModal.open({
					templateUrl: "RolePermissionsModal.html",
					backdrop: "static",
					keyboard: false,
					resolve: {
						data: function() {
							return {
								roleId      : id,
								roleName    : role.roleName,
								securityKeys: res.data,
								allSelected : isAllSelected,
								refreshFunc : $scope.table.refresh
							}
						}
					},
					controller: [
						"$scope", "$uibModalInstance", "data",
						function($scope, $uibModalInstance, data) {
							Object.assign($scope, data);
							$scope.selectAll = function() {
								$scope.securityKeys.forEach(function (key) {
									key.isSelected = $scope.allSelected;
								});
							};

							$scope.cancel = function() {
								if($scope.form.$dirty) {
									var save = window.confirm("You have unsaved changes. Click OK to save, or Cancel to discard.")
									if(save) {
										$scope.save();
										return;
									}
								}
								$uibModalInstance.dismiss();
							};
	
							$scope.save = function() {
								roleService.saveSecurityKeys({ roleId: $scope.roleId, roleSecurityKeys:$scope.securityKeys}, (res) => {
									$scope.refreshFunc(res.data);
									$uibModalInstance.dismiss();
								}, (res) => {
									console.log(res);
								});
							};
						}
					]
				}).result.then(angular.noop, angular.noop);
			}, (res) => {
				console.log(res);
			});
		};

		$scope.addRole = function(data, form) {
			data.unshift({ roleId: -data.length - 1 });
		};

		$scope.save = function (data, form, refreshFunc) {
			roleService.saveRoles(data, (res) => {
				form.$setPristine();
				refreshFunc(res.data);
			}, (res) => {
				console.log(res);
			});
		};
	}
]);