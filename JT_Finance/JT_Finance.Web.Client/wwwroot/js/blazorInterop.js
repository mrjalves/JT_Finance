// Objeto para armazenar as instâncias dos gráficos criados
window.blazorCharts = {};

// Função para criar ou atualizar um gráfico com estilo moderno
window.blazorCharts.createChart = (chartId, type, labels, datasets, title) => {
    const ctx = document.getElementById(chartId);
    if (!ctx) {
        console.error(`Elemento do gráfico com id '${chartId}' não encontrado.`);
        return;
    }

    // Paleta de cores moderna e suave
    const modernColors = [
        '#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b',
        '#858796', '#5a5c69', '#fd7e14', '#6610f2', '#e83e8c'
    ];

    // Adiciona as cores aos datasets se não tiverem sido definidas
    datasets.forEach((ds, index) => {
        ds.backgroundColor = ds.backgroundColor || modernColors;
        ds.borderColor = ds.borderColor || '#ffffff'; // Borda branca para separar fatias
        ds.borderWidth = type === 'pie' || type === 'doughnut' ? 2 : 1;
        ds.borderRadius = type === 'bar' ? 5 : 0; // Bordas arredondadas para gráficos de barra
    });

    // Opções de configuração padrão para todos os gráficos
    const options = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'bottom', // Legenda na parte inferior
                labels: {
                    font: {
                        family: "'Segoe UI', 'Roboto', 'Helvetica Neue', Arial, sans-serif"
                    }
                }
            },
            title: {
                display: true,
                text: title,
                font: {
                    size: 18,
                    family: "'Segoe UI', 'Roboto', 'Helvetica Neue', Arial, sans-serif",
                    weight: '600'
                },
                padding: {
                    top: 10,
                    bottom: 20
                }
            },
            // Configuração do plugin datalabels
            datalabels: {
                // Formata os valores para moeda (R$)
                formatter: (value, context) => {
                    return new Intl.NumberFormat('pt-BR', {
                        style: 'currency',
                        currency: 'BRL'
                    }).format(value);
                },
                color: '#fff',
                font: {
                    weight: 'bold'
                },
                // Esconde labels muito pequenos para não poluir o gráfico
                display: function(context) {
                    var dataset = context.dataset;
                    var count = dataset.data.length;
                    var value = dataset.data[context.dataIndex];
                    return value > 0; // Ou um percentual do total
                }
            }
        }
    };

    // Opções específicas para gráficos que TÊM eixos (barra, linha, etc.)
    if (type === 'bar' || type === 'line') {
        options.scales = {
            x: {
                grid: {
                    display: false // Remove linhas de grade verticais
                },
                ticks: {
                    font: {
                        family: "'Segoe UI', 'Roboto', 'Helvetica Neue', Arial, sans-serif"
                    }
                }
            },
            y: {
                grid: {
                    color: '#e3e6f0', // Linha de grade suave
                    borderDash: [2, 4] // Linha tracejada
                },
                ticks: {
                    // Formata os ticks do eixo Y como moeda
                    callback: function(value, index, values) {
                        return new Intl.NumberFormat('pt-BR', {
                            style: 'currency',
                            currency: 'BRL',
                            notation: 'compact' // Ex: R$ 1,5 mil
                        }).format(value);
                    },
                    font: {
                        family: "'Segoe UI', 'Roboto', 'Helvetica Neue', Arial, sans-serif"
                    }
                }
            }
        };
        // Para gráficos de barra, a legenda é geralmente redundante
        options.plugins.legend.display = datasets.length > 1;
    }

    // Se já existe um gráfico com este ID, destrói antes de criar um novo
    if (window.blazorCharts[chartId]) {
        window.blazorCharts[chartId].destroy();
    }
    
    // Registra o plugin globalmente (necessário apenas uma vez, mas seguro de chamar aqui)
    Chart.register(ChartDataLabels);

    // Cria a nova instância do gráfico
    window.blazorCharts[chartId] = new Chart(ctx, {
        type: type,
        data: {
            labels: labels,
            datasets: datasets
        },
        options: options
    });
};
