$app.service('roleService', ['$http',
	function ($http) {
		var makeRequest = function (method, methodName, params, data, successFunction, errorFunction) {
			$http({
				method: method,
				url: '/api/role/' + methodName,
				params: params,
				data: data,
				headers: { 'Content-Type': 'application/json' }
			}).then(function (response) {
				if (response.status !== null && response.Success === false && errorFunction !== null) {
					errorFunction(response);
				} else if (successFunction) {
					successFunction(response);
				}
			}, function errorCallback(response) {
				if (errorFunction !== null)
					errorFunction(response);
			});
		};

		this.fetchRoles = function (searchCriteria, onSuccess, onFailure) {
			makeRequest("POST", "", null, searchCriteria, onSuccess, onFailure);
		};

		this.fetchKeysForRole = function (id, onSuccess, onFailure) {
			makeRequest("POST", "keys", null, id, onSuccess, onFailure);
		};
	
		this.saveRoles = function (roles, onSuccess, onFailure) {
			makeRequest("POST", "save", null, roles, onSuccess, onFailure);
		};

		this.saveSecurityKeys= function (securityKeysModel, onSuccess, onFailure) {
			makeRequest("POST", "saveKeys", null, securityKeysModel, onSuccess, onFailure);
		};
		return this;
	}
]);