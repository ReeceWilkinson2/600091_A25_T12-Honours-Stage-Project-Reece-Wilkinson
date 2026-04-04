window.renderLineChart = (canvasId, chartPoints, moduleTooltips, counts) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    if (canvas.chartInstance) {
        canvas.chartInstance.destroy();
    }

    // Calculate min and max dates from chartPoints
    const minDate = new Date(Math.min(...chartPoints.map(p => new Date(p.x))));
    const maxDate = new Date(Math.max(...chartPoints.map(p => new Date(p.x))));

    // Optionally, add padding to the date range
    const datePadding = 5 * 24 * 60 * 60 * 1000; // 5 days padding in milliseconds
    minDate.setTime(minDate.getTime() - datePadding);  // Subtract padding from min date
    maxDate.setTime(maxDate.getTime() + datePadding);  // Add padding to max date

    const pointColors = counts.map(count => {
        const t = Math.min(count / 6.0, 1);
        const red = Math.round(255 * t);
        const green = Math.round(200 * (1 - t));
        const blue = 0;
        return `rgb(${red},${green},${blue})`;
    });

    // Ensure chartPoints contains Date objects
    chartPoints = chartPoints.map(p => ({
        x: new Date(p.x),  // Ensure x is a Date object
        y: p.y
    }));

    canvas.chartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            datasets: [{
                label: 'Competency Progress',
                data: chartPoints, // Pass the points as Date objects
                parsing: false,
                borderColor: 'blue',
                backgroundColor: 'blue',
                pointBackgroundColor: pointColors,
                pointBorderColor: pointColors,
                pointRadius: 6,
                pointHoverRadius: 8,
                fill: false,
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const count = context.dataset.data.filter(d => d.x === context.raw.x).length;
                            return `Achievements: ${count}`;
                        },
                        afterLabel: function (context) {
                            const modules = moduleTooltips[context.dataIndex];
                            return modules && modules.length
                                ? ["Modules:", ...modules]
                                : "";
                        }
                    }
                },
                legend: {
                    display: true
                }
            },
            scales: {
                x: {
                    type: 'time',
                    time: {
                        unit: 'day',
                        tooltipFormat: 'yyyy-MM-dd',
                        displayFormats: {
                            day: 'MMM dd, yyyy'
                        }
                    },
                    title: {
                        display: true,
                        text: 'Date First Achieved'
                    },
                    min: minDate, // Set the min date dynamically
                    max: maxDate, // Set the max date dynamically
                },
                y: {
                    min: 3,
                    max: 7,
                    ticks: {
                        stepSize: 1,
                        callback: function (value) {
                            return `Level ${value}`;
                        }
                    },
                    title: {
                        display: true,
                        text: 'Competency Level'
                    }
                }
            }
        }
    });
};