window.renderLineChart = (canvasId, labels, yValues, moduleTooltips, counts) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    if (canvas.chartInstance) {
        canvas.chartInstance.destroy();
    }

    const pointColors = counts.map(count => {
        const t = Math.min(count / 6.0, 1);
        const red = Math.round(255 * t);
        const green = Math.round(200 * (1 - t));
        const blue = 0;
        return `rgb(${red},${green},${blue})`;
    });

    canvas.chartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Competency Progress',
                data: yValues,
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
                            const count = counts[context.dataIndex];
                            return `Achievements: ${count}`;
                        },
                        afterLabel: function (context) {
                            const modules = moduleTooltips[context.dataIndex];
                            return modules && modules.length ? ["Modules:", ...modules] : "";
                        }
                    }
                },
                legend: {
                    display: true
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Date First Achieved'
                    },
                    ticks: {
                        autoSkip: false
                    }
                },
                y: {
                    min: 3,
                    max: 7,
                    ticks: {
                        stepSize: 1,
                        callback: value => `Level ${value}`
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