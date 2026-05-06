$app.service('userService', ['$http',
	function ($http) {
		var makeRequest = function (method, methodName, params, data, successFunction, errorFunction) {
			$http({
				method: method,
				url: '/api/user/' + methodName,
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

		this.fetchUsers = function (searchCriteria, onSuccess, onFailure) {
			makeRequest("POST", "", null, searchCriteria, onSuccess, onFailure);
		};

		this.fetchRoles = function (onSuccess, onFailure) {
			makeRequest("GET", "roles", null, null, onSuccess, onFailure);
		};
	
		this.saveUser = function (user, onSuccess, onFailure) {
			makeRequest("POST", "saveUser", null, user, onSuccess, onFailure);
		};
		this.deleteUser = function (user, onSuccess, onFailure) {
			makeRequest("DELETE", "",null, user, onSuccess, onFailure);
        }

		return this;
	}
]);