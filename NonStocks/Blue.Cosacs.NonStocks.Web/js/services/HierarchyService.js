/*global _, module */
var hierarchyService = function ($http) {
    'use strict';

    var getFilteredValues = function (hierarchy, filterValue) {
        return _.filter(hierarchy, function (l) {
            return l && l.Level && l.Level.Name && l.Level.Name === filterValue;
        });
    };

    var getLevelScopeObject = function (elements, nGet, vGet) {
        var retScopeObject = {};
        _.map(elements, function (elem) {
            var name = nGet(elem);
            var val = vGet(elem);
            retScopeObject[name] = val;
        });
        return retScopeObject;
    };

    var getLevelDataArray = function (elements, nGet, vGet) {
        var retDataArray = _.map(elements, function (elem) {
            var name = nGet(elem);
            var val = vGet(elem);
            return {"Key": name, "Name": val};
        });
        return retDataArray;
    };

    var processMerchandising = function (scope, allLevels, allTags) {
        var retAllLevels = [];
        var retHierarchy = [];

        _.map(allLevels, function (level) {
            var levelName = level.Name;
            var tmpLevel = getFilteredValues(allTags, levelName);

            // creation of the scope objects
            var tmpLevelScope = getLevelScopeObject(tmpLevel,
                function (el) { return el.Id; },  // nGet
                function (el) { return el.Name; } // vGet
            );
            retHierarchy.push({
                "name": levelName,
                "val": null,
                "dataScope": tmpLevelScope
            });

            // creation of the all levels array
            var tmpLevelData = getLevelDataArray(tmpLevel,
                function (el) { return el.Id; },  // nGet
                function (el) { return el.Name; } // vGet
            );
            retAllLevels.push({
                "Name": levelName,
                "Data": tmpLevelData
            });
        });

        scope.formData.levels = retHierarchy;
        scope.formData.allLevels = retAllLevels;
    };

    return {
        getHierarchy: function (scope) {
            $http({
                url: '/Cosacs/Merchandising/Hierarchy/GetHierarchyData',
                method: "GET"
            })
                .then(function (response) {
                    if (response && response.data &&
                        _.isArray(response.data.Tags) && response.data.Tags.length > 0) {
                        if (scope && !scope.formData) {
                            scope.formData = {};
                        }

                        var allLevels = response.data.Levels;
                        var allTags = response.data.Tags;

                        // Build Hierarchy Levels
                        processMerchandising(scope, allLevels, allTags);

                        for (var lev in scope.formData.hierarchy) {
                            if (scope.formData.hierarchy.hasOwnProperty(lev)) {
                                var tmpHierarchy = scope.formData.hierarchy[lev];
                                var tmpLevel = scope.formData.levels[lev];
                                if (tmpLevel) {
                                    tmpLevel.val = tmpHierarchy.SelectedKey || null;
                                }
                            }
                        }

                        scope.formData.ErrorLoadingHierarchy = false;
                    } else {
                        // Error loading non-stocks categories hierarchy!
                        scope.formData.ErrorLoadingHierarchy = true;
                    }
                });
        }
    };
};

hierarchyService.$inject = ['$http'];
module.exports = hierarchyService;
