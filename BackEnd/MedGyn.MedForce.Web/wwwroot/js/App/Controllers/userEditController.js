$app.controller("userEditController", [
	"$scope", "$rootScope", "$uibModalInstance", "userService", "data",
	function ($scope, $rootScope, $uibModalInstance, userService, data) {
		Object.assign($scope, data);

		var disabled = !$scope.user.isDeleted
		$scope.disableBtn = disabled;
		

		$scope.clearInvalid = function() {
			$scope.form.email.$setValidity('unique', true);
		}

		$scope.save = function () {
			if($scope.form.$invalid) {
				$rootScope.showInvalidFields($scope.form);
				return;
			}

			userService.saveUser($scope.user,
				function (res) {
					if(res.data.success) {
						$scope.refreshFunc();
						$uibModalInstance.close();
					} else {
						if(res.data.errorMessage === "DUP_ID") {
							$scope.form.email.$setValidity('unique', false);
						} else {
							alert("Error saving User. Check console for details.");
							console.error(res.data.errorMessage, res.data.fullError);
						}
					}
				},
				function (error) {
					console.log(error);
				 });
		};

		$scope.delete = function () {

			userService.deleteUser($scope.user,
				function (res) {
					if (res.data.success) {
						$scope.refreshFunc();
						$uibModalInstance.close();
					} else {
						if (res.data.errorMessage === "DUP_ID") {
							$scope.form.email.$setValidity('unique', false);
						} else {
							alert("Error saving User. Check console for details.");
							console.error(res.data.errorMessage, res.data.fullError);
						}
					}
				},
				function (error) {
					console.log(error);
				});
		}

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


		//document.getElementById("deletebtn").style.display = $scope.user.isDeleted ? "block" : "none";
	}
]);