$app.directive('numericValue', function () {
	return {
		restrict: "A",
		require: "ngModel",
		link: function (scope, el, attrs, ngModel) {
			el.on("keydown", function(e) {
				var key = e.key
				if(key && key.length === 1 && !key.match(/[\d\.\-]/)) {
					e.preventDefault();
				}
			});

			ngModel.$parsers.push(function(value) {
				const num = parseFloat(value);
				return isNaN(num) ? null : num;
			});
		},
	};
});

// This allow for only inputing numbers, but doesn't convert the value, like above
$app.directive('numbersOnly', function () {
	return {
		restrict: "A",
		require: "ngModel",
		link: function (scope, el,) {
			el.on("keydown", function(e) {
				var key = e.key
				if(key.length === 1 && !key.match(/[\d\.\-]/)) {
					e.preventDefault();
				}
			});
		},
	};
});