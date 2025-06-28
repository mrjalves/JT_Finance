window.blazorCharts = {
    charts: {}, // Para armazenar instâncias de gráficos e permitir atualizações/destruição

    createPieChart: function (canvasId, labels, dataValues, backgroundColors, title) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) {
            console.error("Canvas element not found: " + canvasId);
            return;
        }

        // Se já existe um gráfico nesta tela, destrua-o primeiro
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        this.charts[canvasId] = new Chart(ctx.getContext('2d'), {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    label: title, // Usado na legenda e tooltips
                    data: dataValues,
                    backgroundColor: backgroundColors,
                    borderColor: backgroundColors.map(color => color.replace('0.7', '1')), // Borda mais opaca
                    borderWidth: 1
                }]
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
                        text: title
                    }
                }
            }
        });
    },

    updatePieChart: function (canvasId, newLabels, newDataValues, newBackgroundColors, newTitle) {
        const chart = this.charts[canvasId];
        if (chart) {
            chart.data.labels = newLabels;
            // Supondo que há apenas um dataset para gráficos de pizza
            if (chart.data.datasets.length > 0) {
                chart.data.datasets[0].data = newDataValues;
                chart.data.datasets[0].backgroundColor = newBackgroundColors;
                chart.data.datasets[0].borderColor = newBackgroundColors.map(color => color.replace('0.7', '1'));
                chart.data.datasets[0].label = newTitle; // Atualiza o label do dataset se necessário
            }

            if (chart.options.plugins.title) {
                chart.options.plugins.title.text = newTitle;
            } else {
                 chart.options.plugins.title = { display: true, text: newTitle };
            }
            chart.update();
        } else {
            // Se o gráfico não existir (por exemplo, se os dados chegam antes da primeira renderização completa),
            // tentamos criá-lo. Isso pode precisar de lógica mais robusta dependendo do ciclo de vida.
            // console.warn("Chart instance not found for update, creating new one: " + canvasId);
            // this.createPieChart(canvasId, newLabels, newDataValues, newBackgroundColors, newTitle);
            // É melhor garantir que createPieChart seja chamado primeiro pelo OnParametersSetAsync ou OnAfterRenderAsync.
            // Se updatePieChart for chamado antes de createPieChart ter sido executado pelo menos uma vez,
            // pode haver problemas. O componente Blazor deve gerenciar isso.
        }
    }
};
