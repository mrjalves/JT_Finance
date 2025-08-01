// Objeto para armazenar as instâncias dos gráficos criados
window.blazorCharts = {};

// Função para criar um novo gráfico
window.blazorCharts.createChart = (chartId, type, labels, datasets, title) => {
    const ctx = document.getElementById(chartId).getContext('2d');

    // Se já existe um gráfico com este ID, destrói antes de criar um novo
    if (window.blazorCharts[chartId]) {
        window.blazorCharts[chartId].destroy();
    }

    window.blazorCharts[chartId] = new Chart(ctx, {
        type: type, // 'bar', 'line', 'pie', etc.
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: title,
                    font: {
                        size: 18
                    }
                }
            },
            // --- INÍCIO DA MUDANÇA PARA REMOVER AS LINHAS ---
            scales: {
                x: {
                    grid: {
                        display: false // Desativa as linhas de grade do eixo X
                    }
                },
                y: {
                    grid: {
                        display: false // Desativa as linhas de grade do eixo Y
                    }
                }
            }
            // --- FIM DA MUDANÇA ---
        }
    });
};