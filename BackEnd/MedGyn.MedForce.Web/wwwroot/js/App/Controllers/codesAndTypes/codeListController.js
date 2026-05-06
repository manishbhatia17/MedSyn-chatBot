$app.controller("codeListController", [
	"$scope", "$rootScope", "codeService",
	function ($scope, $rootScope, codeService) {
		var _codeTypeID;
		$scope.init = function(codeTypeID) {
			$scope.canEdit = $rootScope.checkClaims([SecurityKeys.Codes]);
			if(!$scope.canEdit)
				return;
			_codeTypeID = codeTypeID;

			codeService.fetchCodeTypes({}, (res) => {
				$scope.codeType = res.data.results.find(x => x.codeTypeID === codeTypeID);

				$scope.table = {
					idField: "codeID",
					hideAdd: $scope.codeType.lockCodes,
					dataColumns: [
						{ title: "Name", field: "codeName", required: true, editable: (val, row) => !row.isRequired },
						{ title: "Description", field: "codeDescription", editable: true },
						{ title: "Disable Code", field: "isDeleted", type: "checkbox", editable: (val, row) => !row.isRequired },
					],
					additionalFilters: [
						{ type: "static", parameter: "codeTypeID", value: codeTypeID },
					]
				};
			});
		}

		$scope.save = function(data, form, refreshFunc) {
			codeService.saveCodes(data, {codeTypeID: _codeTypeID}, (res) => {
				form.$setPristine();
				refreshFunc(res.data);
			});
		}

		$scope.add = function(data, form) {
			data.unshift({ codeID: -data.length - 1, codeTypeID: _codeTypeID });
		}
	}
]);