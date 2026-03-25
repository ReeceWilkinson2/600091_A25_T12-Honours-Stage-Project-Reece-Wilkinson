window.renderLineChart = (canvasId, labels, data, moduleTooltips) => {

    const canvas = document.getElementById(canvasId);
    const ctx = canvas?.getContext('2d');
    if (!ctx) {
        console.error('Canvas context not found!');
        return;
    }

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Competency Progress',
                data: data,
                borderColor: 'blue',
                fill: false,
                tension: 0.1
            }]
        },
            options: {
                responsive: true,
                plugins: {
                    tooltip: {
                        callbacks: {
                            afterLabel: function (context) {
                                const modules = moduleTooltips[context.dataIndex];
                                return modules && modules.length ? ["Modules:", ...modules] : "";
                            }
                        }
                    }
                },
            scales: {
                y: {
                    min: 3,
                    max: 7,
                    ticks: {
                        stepSize: 1,
                        callback: function (value) {
                            return 'Level ' + value;
                        }
                    },
                    title: { display: true, text: 'Level' }
                },
                x: {
                    title: { display: true, text: 'Date Achieved' }
                }
            }
        }
    });
};