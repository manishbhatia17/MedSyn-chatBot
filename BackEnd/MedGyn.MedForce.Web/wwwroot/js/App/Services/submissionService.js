$app.service('submissionService', ['$http',
    function ($http) {
        var makeRequest = function (method, methodName, params, data, successFunction, errorFunction) {
            $http({
                method: method,
                url: '/api/submission/' + methodName,
                params: params,
                data: data,
                headers: { 'Content-Type': 'application/json' }
            }).then(function successCallback(response) {
                if (response.Success !== null && response.Success === false && errorFunction !== null) {
                    errorFunction(response);
                } else if (successFunction) {
                    successFunction(response);
                }
            }, function errorCallback(response) {
                if (errorFunction !== null)
                    errorFunction(response);
            });
        };

        this.getNewSubmission = function (onSuccess, onFailure) {
            makeRequest("GET", "getNewSubmission", null, null, onSuccess, onFailure);
        };

        this.saveSubmission = function (data, onSuccess, onFailure) {
            makeRequest("POST", "saveSubmission", null, data, onSuccess, onFailure);
        };

        this.getAgentInformation = function (data, onSuccess, onFailure) {
            makeRequest("POST", "getAgentInformation", null, data, onSuccess, onFailure);
        };

        return this;
    }
]);