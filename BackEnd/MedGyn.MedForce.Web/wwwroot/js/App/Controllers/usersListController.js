$app.controller("usersListController", [
	"$scope", "$rootScope", "$uibModal", "userService", 
	function ($scope, $rootScope, $uibModal, userService) {	
	$scope.canEdit = $rootScope.checkClaims([SecurityKeys.UserEdit]);
	$scope.table   = {
		idField    : "userId",
		hideAdd    : !$scope.canEdit,
		hideSave   : !$scope.canEdit,
		showEdit: $scope.canEdit,
		hideUpload: true,
		hidePriceImport: true,
		dataColumns: [
			{ title: "First Name", field: "firstName", editable: false, maxlength: 100},
			{ title: "Last Name", field: "lastName", editable: false, maxlength: 100},
			{ title: "Email", field: "email", editable: false, maxlength: 500},
			{ title: "Rep ID", field: "salesRepId", editable: false, maxlength: 100},
			{ title: "Role", field: "roleName", editable: false}
		]
	};


	$scope.editClicked = function (userId, user) {
		var editUser = {};
		if(userId !== 0)
			Object.assign(editUser, user);
		$uibModal.open({
			templateUrl: 'UserEdit.html',
			controller : 'userEditController',
			backdrop: "static",
			keyboard   : false,
			resolve    : {
				userService: userService,
				data       : function() {
					return {
						user       : editUser,
						roles      : $scope.roles,
						refreshFunc: $scope.table.refresh
					}
				}
			}
		}).result.then(angular.noop, angular.noop);
	};

	$scope.getRoles = function () {
		userService.fetchRoles((res) => {
			$scope.roles = res.data;
		}, (res) => {
			console.log(res);
		});
	};
}]);