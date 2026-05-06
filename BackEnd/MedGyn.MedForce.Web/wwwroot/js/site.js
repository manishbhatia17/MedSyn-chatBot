function isFunction(thing){
	return typeof thing === "function";
}

function downloadByteArray(bytes, filename, fileType){
	var a = document.createElement("a");
	a.setAttribute("href", `data:${fileType};base64,${bytes}`);
	a.setAttribute("download", filename);
	a.click();
}

var $app;
$app = angular.module('app', ['ui.bootstrap', 'ngMessages', 'ngMask', 'ngSanitize'])

// Don't let angular cache api calls
$app.config(["$httpProvider", function($httpProvider) {
	if(!$httpProvider.defaults.headers.get) {
		$httpProvider.defaults.headers.get = {};
	}
	$httpProvider.defaults.headers.get["Cache-Control"] = "no-cache";
}]);

$app.filter("absValue", function() {
	return function (num) { return Math.abs(num); }
})

$app.filter("localTime", ["$filter", function($filter) {
	return function(utcTimeStr, format) {
		if(!utcTimeStr)
			return;
		if(!format)
			format = "MM/dd/yyyy HH:mm:ss";

		var utcTime = new Date(utcTimeStr);
		var localTime = utcTime.setTime(utcTime.getTime() - utcTime.getTimezoneOffset() * 6e4);
		return $filter("date")(localTime, format);
	}
}]);

$app.filter("codeFilter", [function() {
	return function(codes, existingValue) {
		var filteredCodes = codes?.filter(x => {
			return x.visible || x.value === existingValue;
		});
		return filteredCodes;
	}
}]);
