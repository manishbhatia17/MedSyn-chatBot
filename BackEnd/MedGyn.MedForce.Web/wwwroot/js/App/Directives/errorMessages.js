$app.directive('errorMessages', ["$interpolate", function ($interpolate) {
	return {
		restrict: "E",
		template: `
			<div class="error-messages" id="{{field}}_errors" ng-messages="form[field].$error" ng-show="form[field].$touched || form[field].$dirty">
				<div ng-message="required">{{ requiredMsg }}</div>
				<div ng-message="maxlength">{{ maxlengthMsg }}</div>
				<div ng-message="pattern">{{ patternMsg }}</div>
				<div ng-message="number">{{ numberMsg }}</div>
				<div ng-message="mask">{{ maskMsg }}</div>
				<div ng-message="min">{{ minMsg }}</div>
				<div ng-message="max">{{ maxMsg }}</div>
				<div ng-message="unique">{{ uniqueMsg }}</div>
				<div ng-message-default>Invalid input</div>
			</div>
		`,
		scope: true,
		controller: [
			"$scope",
			function($scope) {

			}
		],

		link: function (scope, el, attrs, ngModel) {
			var formElem = el.closest("form");
			scope.form = formElem.scope()[formElem[0].attributes.name.value];
			// scope.form = scope.$parent.form;
			if(attrs.field) {
				scope.field = attrs.field
			} else {
				scope.field = $interpolate(el[0].previousElementSibling.attributes.name.value)(scope);
			}

			scope.requiredMsg  = attrs.requiredMsg ? attrs.requiredMsg : "This field is required";
			scope.maxlengthMsg = attrs.maxlengthMsg ? attrs.maxlengthMsg : "Value is too long";
			scope.patternMsg   = attrs.patternMsg ? attrs.patternMsg : "Invalid format";
			scope.numberMsg    = attrs.numberMsg ? attrs.numberMsg : "Value is not a number";
			scope.maskMsg      = attrs.maskMsg ? attrs.maskMsg : "Invalid format";
			scope.minMsg       = attrs.minMsg ? attrs.minMsg : "Value is too small";
			scope.maxMsg       = attrs.maxMsg ? attrs.maxMsg : "Value is too big";
			scope.uniqueMsg    = attrs.uniqueMsg ? attrs.uniqueMsg : "Value must be unique";
		},
	};
}]);