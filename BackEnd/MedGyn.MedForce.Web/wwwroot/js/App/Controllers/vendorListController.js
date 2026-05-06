$app.controller("vendorListController", [
	"$scope", "$rootScope", "vendorService",
	function ($scope, $rootScope) {
		$scope.canView = $rootScope.checkClaims([SecurityKeys.VendorView]);
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.VendorEdit]);
		$scope.table   = {
			idField: "vendorID",
			showEdit: $scope.canEdit || $scope.canView,
			hideAdd: !$scope.canEdit,
			hideUpload: true,
			hidePriceImport: true,
			dataColumns: [
				{ title: "Name", field: "vendorName" },
				{ title: "Vendor ID", field: "vendorCustomID" },
				{ title: "City", field: "city" },
				{ title: "Country", field: "countryCodeID" },
				{ title: "Status", field: "vendorStatusCodeID" }
			]
		};
	}
]);