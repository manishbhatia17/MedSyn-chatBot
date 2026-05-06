$app.controller("appController", ["$scope", "$rootScope", function ($scope, $rootScope) {

	$rootScope.showInvalidFields = function(form) {
		angular.forEach(form.$error, function (field) {
			angular.forEach(field, function (errorField) {
				errorField.$setTouched();
				errorField.$validate();
			});
		});
		alert("There are required or invalid fields.");
	}
}]);