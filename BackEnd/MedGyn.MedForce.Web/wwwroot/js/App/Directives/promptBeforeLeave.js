$app.directive('promptBeforeLeave', function () {
	return {
		restrict: "A",
		link: function (scope, el, attrs) {
			var form = scope[attrs.name];
			if(!form) {
				return;
			}

			window.addEventListener("beforeunload", function(event) {
				if(form.$dirty) {
					// there should be only 1 browser. This is stupid.
					event.returnValue = "There are unsaved changes"; // Chrome, Edge, Firefox (deprecated)
					event.preventDefault(); // Firefox, IE, Safari 11+ (not deprecated, but not supported by Chrome/Edge)
					return "There are unsaved changes"; // Old browsers (deprecated)
				}
			});
		},
	};
});