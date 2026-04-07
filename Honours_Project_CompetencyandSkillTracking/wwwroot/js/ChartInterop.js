window.renderLineChart = (canvasId, labels, yValues, moduleTooltips, counts) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    if (canvas.chartInstance) {
        canvas.chartInstance.destroy();
    }

    // RGB color transitions from (0, 238, 255) to (0, 0, 255)
    const rgbColors = [
        [0, 238, 255],  // Level 1
        [0, 190, 255],  // Level 2
        [0, 142, 255],  // Level 3
        [0, 94, 255],  // Level 4
        [0, 46, 255],   // Level 5
        [0, 0, 255]]  // Level 6

    const pointColors = counts.map(count => {
        const index = Math.min(Math.max(count - 1, 0), 5);
        const color = rgbColors[index];
        return `rgb(${color[0]}, ${color[1]}, ${color[2]})`;  // Convert to rgb string
    });

    canvas.chartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Competency Progress',
                data: yValues,
                borderColor: 'black',
                backgroundColor: 'black',
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