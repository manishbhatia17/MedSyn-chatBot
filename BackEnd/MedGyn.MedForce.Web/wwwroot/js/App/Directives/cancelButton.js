$app.directive('cancelButton', function () {
	return {
		restrict: "E",
		template: `<button type="button" ng-click="cancel()" class="btn btn-primary">Cancel</button>`,
		scope: {
			page: "@",
			form: "=",
			save: "&",
		},
		replace: true,
		controller: [
			"$scope",
			function ($scope) {
				$scope.cancel = function () {
					if($scope.form.$dirty) {
						var save = window.confirm("You have unsaved changes. Click OK to save, or Cancel to discard.")
						if(save) {
							$scope.save();
							return;
						} else {
							$scope.form.$setPristine();
						}
					}
					location.href = $scope.page;
				};
			}
		],
	};
});