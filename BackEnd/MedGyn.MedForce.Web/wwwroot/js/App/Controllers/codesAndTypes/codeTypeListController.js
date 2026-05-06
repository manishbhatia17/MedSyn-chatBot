$app.controller("codeTypeListController", [
	"$scope", "$rootScope", "codeService",
	function ($scope, $rootScope, codeService) {
		$scope.canEdit = $rootScope.checkClaims([SecurityKeys.CodeTypes]);
		$scope.hasCodes = $rootScope.checkClaims([SecurityKeys.Codes]);
		$scope.table = {
			idField: "codeTypeID",
			hideAdd: true,
			hideSave: !$scope.canEdit,
			dataColumns: [
				{ title: "Code Type ID", field: "codeTypeID" },
				{ title: "Name", field: "codeTypeName", editable: $scope.canEdit, maxlength: 100, },
			],
			actionColumns: [
				{ title: "Codes", func: (id) => location.href = `/CodeType/${id}/codes`, hide: !$scope.hasCodes }
			]
		};

		$scope.save = function (data, form, refreshFunc) {
			codeService.saveCodeTypes(data, (res) => {
				form.$setPristine();
				refreshFunc(res.data);
			});
		}
	}
]);