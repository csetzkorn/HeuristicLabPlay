﻿(function () {
    var module = appPluginsPlugin.getAngularModule();
    module.controller('app.plugins.ctrl',
        ['$scope', '$modal', 'app.plugins.service', function ($scope, $modal, pluginService) {
            var getPlugins = function () {
                pluginService.getPlugins({}, function (plugins) {
                    $scope.plugins = plugins;
                    var length = $scope.plugins.length;
                    for (var i = 0; i < length; ++i) {
                        if (!isDefined($scope.plugins[i].AssemblyName)) {
                            $scope.plugins[i].AssemblyName = 'Not found';
                        }
                        var datetime = $scope.plugins[i].LastReload;
                        if (isDefined(datetime)) {
                            $scope.plugins[i].LastReload = CSharpDateToString(datetime);
                        } else {
                            $scope.plugins[i].LastReload = 'Never';
                        }
                    }
                });
            };

            $scope.reloadPlugin = function (name) {
                pluginService.reloadPlugin({ name: name }, function () {
                    getPlugins();
                });
            };

            $scope.open = function (pluginName, exception) {
                $scope.pluginName = pluginName;
                $scope.exception = exception;
                $modal.open({
                    animation: true,
                    templateUrl: 'pluginsExceptionDialog',
                    controller: 'app.plugins.pluginsExceptionCtrl',
                    resolve: {
                        pluginName: function () {
                            return $scope.pluginName;
                        },
                        exception: function () {
                            return $scope.exception;
                        }
                    }
                });
            };

            getPlugins();
        }]
    );
})();