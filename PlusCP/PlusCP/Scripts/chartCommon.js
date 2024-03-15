//// For Average Cost 
//function DrawMeChart(chartId, data, title, chartType) {
//    chartId.data = [];
//    window.myBar = new Chart(chartId, {
//        type: chartType,
//        data: data,
//        options: {
//            responsive: true,
//            title: {
//                display: true,
//                text: title,
//            },
//            tooltips: {
//                mode: 'index',
//                intersect: false,
//                callbacks: { // for thousand seperator for tooltip on bar
//                    label: function (tooltipItem, data) {
//                        var tooltipValue = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
//                        return thousands_separators(Math.round(tooltipValue));
//                    }
//                }
//            },
//            responsive: true,
//            scales: {
//                xAxes: [{
//                    ticks: {
//                        beginAtZero: true,                  
//                        stepSize: 0, // It shows values under range
//                        callback: function (value) {
//                            var ranges = [
//                                { divider: 1e6, suffix: 'M' },
//                                { divider: 1e3, suffix: 'k' }
//                            ];
//                            function formatNumber(n) {
//                                for (var i = 0; i < ranges.length; i++) {
//                                    if (n >= ranges[i].divider) {
//                                        return (n / ranges[i].divider).toString() + ranges[i].suffix;
//                                    }
//                                }
//                                return n;
//                            }
//                            return '' + formatNumber(value);
//                        }
//                    }
//                }],
//                yAxes: [{
//                    stacked: false
//                }]
//            }
//        }
//    });
//}