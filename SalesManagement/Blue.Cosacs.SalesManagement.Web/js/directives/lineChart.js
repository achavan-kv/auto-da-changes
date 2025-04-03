'use strict';

var lineChart = function ($compile) {
    return {
        restrict: 'E',
        scope: {
            width: '@',
            height: '@',
            xproperty: '@',
            ylabel: '@',
            title: '@',
            source: '=source'
        },
        link: function (scope, element) {
            var layoutSize = 1170;
            var width = 0;
            var height = 0;

            if (_.isUndefined(scope.width)){
                width = layoutSize - 60 - 70; /*margin.left - margin.right*/
            }
            else{
                width = _.parseInt(scope.width)- 60 - 70; /*margin.left - margin.right*/;
            }

            if (_.isUndefined(scope.height)){
                height = 500 - 10 - 40; /* margin.top - margin.bottom*/
            }
            else{
                height = _.parseInt(scope.height) - 10 - 40; /* margin.top - margin.bottom*/;
            }

            var localConfig = _.extend({}, {
                xAxisProperty: scope.xproperty,
                yAxisLabel: scope.ylabel,
                margin: { top: 10, right: 70, bottom: 40, left: 60 },
                chartTitle: scope.title,
//                tooltipBuilder: {
//                    title: null,
//                    render: function (d) {
//                        return localConfig.xAxisProperty + ": " + d.label +
//                        "<br/>" + d.name.replace(/(?=[A-Z])/g, ' ').trim() + ": " + d3.format(",")(d.value || d.value === 0 ? d.value : d.y1 - d.y0);
//                    }
//                },
                width: width,
                height: height,
                colors: d3.scale.ordinal().range(["#101b4d", "#475003", "#9c8305", "#B6DBB6", "#bAE1ED", "#DFEFFC", "#001c9c", "#d3c47c", "#e6e6e6", "#F7D2F2", "#c5cde5", "#DCD2B4"])
            });

            element.html($compile('<svg width="' + localConfig.width + '" height="' + localConfig.height + '"></svg>')(scope));

            var svg = element.find('svg')[0];

            function clear() {
                d3.select(svg).selectAll("*").remove();
                //d3.select(svg).remove();
            }

            function render(data, svgElement) {
                clear();
                var x = d3.scale.ordinal().rangeRoundBands([0, localConfig.width], 0.1);

                var y = d3.scale.linear().rangeRound([localConfig.height, 0]);

                var xAxis = d3.svg.axis()
                    .scale(x)
                    .orient("bottom");

                var yAxis = d3.svg.axis()
                    .scale(y)
                    .orient("left");

                var color = localConfig.colors;

                var svg = d3.select(svgElement)
                    //.attr("id", "thesvg")
                    .attr("width", localConfig.width + localConfig.margin.left + localConfig.margin.right)
                    .attr("height", localConfig.height + localConfig.margin.top + localConfig.margin.bottom)
                    .append("g")
                    .attr("transform", "translate(" + localConfig.margin.left + "," + localConfig.margin.top + ")");

                function drawLegend(varNames) {
                    var varNamesCase = [];

                    _.each(varNames, function (current) {
                        varNamesCase.unshift(current.replace(/(?=[A-Z])/g, ' ').trim());
                    });


                    var legend = svg.selectAll(".legend")
                        .data(varNamesCase.slice())
                        .enter().append("g")
                        .attr("class", "legend")
                        .attr("transform", function (d, i) {
                            return "translate(55," + i * 20 + ")";
                        });

                    legend.append("rect")
                        .attr("x", localConfig.width - 10)
                        .attr("width", 10)
                        .attr("height", 10)
                        .style("fill", color)
                        .style("stroke", "grey");

                    legend.append("text")
                        .attr("x", localConfig.width - 12)
                        .attr("y", 6)
                        .attr("dy", ".35em")
                        .style("text-anchor", "end")
                        .text(function (d) {
                            return d;
                        });
                }

                /*
                 function removePopovers() {
                 $('.popover').each(function () {
                 $(this).remove();
                 });
                 }

                 function showPopover(d) {
                 $(this).popover({
                 title: localConfig.tooltipBuilder.title || d.name,
                 placement: 'auto top',
                 container: 'body',
                 trigger: 'manual',
                 html: true,
                 content: localConfig.tooltipBuilder.render(d)
                 });
                 $(this).popover('show');
                 }
                 */
                var line = d3.svg.line()
                    .interpolate("cardinal")
                    .x(function (d) {
                        return x(d.label) + x.rangeBand() / 2;
                    })
                    .y(function (d) {
                        return y(d.value);
                    });

                var varNames = d3.keys(data[0]).filter(function (key) {
                    return key !== localConfig.xAxisProperty;
                });
                color.domain(varNames);

                var seriesData = varNames.map(function (name) {
                    return {
                        name: name,
                        values: data.map(function (d) {
                            return {name: name, label: d[localConfig.xAxisProperty], value: +d[name]};
                        })
                    };
                });

                x.domain(data.map(function (d) {
                    return d[localConfig.xAxisProperty];
                }));

                y.domain([
                    d3.min(seriesData, function (c) {
                        return d3.min(c.values, function (d) {
                            return d.value;
                        });
                    }),
                    d3.max(seriesData, function (c) {
                        return d3.max(c.values, function (d) {
                            return d.value;
                        });
                    })
                ]);

                //Add a title to the chart
                svg.append("text")
                    .attr("x", localConfig.width / 2)
                    .attr("y", localConfig.margin.top / 2)
                    .attr("text-anchor", "middle")
                    .style("font-size", "16px")
                    .style("text-decoration", "underline")
                    .text(localConfig.chartTitle);

                var xHeight = localConfig.height + 20;

                //create the x axis
                svg.append("g")
                    .attr("class", "x axis")
                    .attr("transform", "translate(0," + xHeight + ")")
                    .call(xAxis);

                //create y axis
                svg.append("g")
                    .attr("class", "y axis")
                    .attr("transform", "translate(0, 20)")
                    .call(yAxis)
                    .append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("y", 6)
                    .attr("dy", ".71em")
                    .style("text-anchor", "end")
                    .text(localConfig.yAxisLabel);

                //create the object that holds the elements inside the chart
                var series = svg.selectAll(".series")
                    .data(seriesData)
                    .enter().append("g")
                    .attr("class", "series")
                    .attr("transform", "translate(0, 20)");

                //draw the main line in the chart
                series.append("path")
                    .attr("class", "line")
                    .attr("d", function (d) {
                        return line(d.values);
                    })
                    .style("stroke", function (d) {
                        return color(d.name);
                    })
                    .style("stroke-width", "3px")
                    .style("fill", "none");

                //Put the circles in the lines
                series.selectAll(".linePoint")
                    .data(function (d) {
                        return d.values;
                    })
                    .enter().append("circle")
                    .attr("class", "linePoint")
                    .attr("cx", function (d) {
                        return x(d.label) + x.rangeBand() / 2;
                    })
                    .attr("cy", function (d) {
                        return y(d.value);
                    })
                    .attr("r", "5px")
                    .style("fill", function (d) { 
                        return color(d.name);
                    })
                    .style("stroke", "grey")
                    .style("stroke-width", "1px");
                /*
                 .on("mouseover", function (d) {
                 showPopover.call(this, d);
                 })
                 .on("mouseout", function () {
                 removePopovers();
                 });
                 */

                //put a horizontal line inside chart
                svg.selectAll('.y.axis g line')
                    .attr("x2", localConfig.width)
                    .attr("x1", "-6");

                drawLegend(varNames);
            }

            scope.$watch('source', function(newValue){
                if (_.isNull(newValue) || _.isUndefined(newValue)){
                    clear();
                }
                else {
                    render(newValue, svg);
                }
            });
        }
    };
};

lineChart.$inject = ['$compile'];
module.exports = lineChart;