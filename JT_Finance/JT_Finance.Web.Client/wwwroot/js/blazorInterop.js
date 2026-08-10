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
                display: function (context) {
                    var dataset = context.dataset;
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
                    callback: function (value, index, values) {
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

// ============================================================
// appHotkeys — Configuração dinâmica de atalhos de teclado
// ============================================================
window.appHotkeys = (function () {
    let config = {
        search: 'Ctrl+K',
        filter: 'F3',
        novo: 'Alt+N',
        editar: 'Alt+E',
        deletar: 'Alt+D',
        salvar: 'Alt+S',
        cancelar: 'Esc',
        ia: 'Alt+I'
    };

    let mainRef = null;
    let pageRef = null;
    let dispatchHandler = null;
    let capture = null;
    let capturing = false;

    function isTypingContext() {
        const ae = document.activeElement;
        if (!ae) return false;
        const tag = ae.tagName;
        const isFormField = tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT';
        return isFormField || ae.isContentEditable === true;
    }

    function parseCombo(combo) {
        const p = { ctrl: false, alt: false, shift: false, meta: false, key: null };
        if (!combo) return p;
        for (const raw of String(combo).split('+')) {
            const s = raw.trim();
            if (!s) continue;
            if (s === 'Ctrl') p.ctrl = true;
            else if (s === 'Alt') p.alt = true;
            else if (s === 'Shift') p.shift = true;
            else if (s === 'Meta') p.meta = true;
            else p.key = s;
        }
        return p;
    }

    function keyLabel(k) {
        if (k === 'Esc') return 'Escape';
        if (k === 'Del') return 'Delete';
        if (k === 'Space') return ' ';
        if (k === 'Plus') return '+';
        return k;
    }

    function keysEqual(evtKey, comboKey) {
        const c = keyLabel(comboKey);
        if (!c) return false;
        if (evtKey && evtKey.length === 1 && c.length === 1) {
            return evtKey.toLowerCase() === c.toLowerCase();
        }
        return evtKey === c;
    }

    function matches(e, combo) {
        const p = parseCombo(combo);
        if (p.key == null) return false;
        return e.ctrlKey === p.ctrl &&
               e.altKey === p.alt &&
               e.shiftKey === p.shift &&
               e.metaKey === p.meta &&
               keysEqual(e.key, p.key);
    }

    function displayKey(key) {
        if (!key) return '';
        if (key === ' ') return 'Space';
        if (key.length === 1) return key.toUpperCase();
        const map = {
            Escape: 'Esc', Delete: 'Del',
            ArrowUp: 'Up', ArrowDown: 'Down', ArrowLeft: 'Left', ArrowRight: 'Right',
            Backspace: 'Backspace', Tab: 'Tab', Enter: 'Enter',
            Insert: 'Ins', Home: 'Home', End: 'End', PageUp: 'PgUp', PageDown: 'PgDn'
        };
        if (map[key]) return map[key];
        if (/^F\d{1,2}$/.test(key)) return key;
        return key;
    }

    function buildCombo(e) {
        const parts = [];
        if (e.ctrlKey) parts.push('Ctrl');
        if (e.altKey) parts.push('Alt');
        if (e.shiftKey) parts.push('Shift');
        if (e.metaKey) parts.push('Meta');
        const k = displayKey(e.key);
        if (k) parts.push(k);
        return parts.join('+');
    }

    function safeCall(ref, method, arg) {
        if (!ref) return;
        try {
            const p = arg === undefined ? ref.invokeMethodAsync(method) : ref.invokeMethodAsync(method, arg);
            if (p && typeof p.catch === 'function') p.catch(() => { /* no-op */ });
        } catch { /* no-op */ }
    }

    function invokeAcao(acao) {
        switch (acao) {
            case 'search': safeCall(mainRef, 'HandleSearchHotkey'); break;
            case 'filter': safeCall(mainRef, 'HandleFilterHotkey'); break;
            case 'ia':     safeCall(mainRef, 'HandleIAHotkey'); break;
            case 'novo':   safeCall(pageRef, 'OnNovoHotkey'); break;
            default:       safeCall(pageRef, 'HandleAcaoHotkey', acao); break;
        }
    }

    function handleKey(e) {
        if (capturing) return;

        if (isTypingContext()) {
            // Em campos de digitação, apenas Salvar e Cancelar ficam ativos.
            if (config.salvar && matches(e, config.salvar)) {
                e.preventDefault();
                invokeAcao('salvar');
            } else if (config.cancelar && matches(e, config.cancelar)) {
                e.preventDefault();
                invokeAcao('cancelar');
            }
            return;
        }

        const order = ['search', 'filter', 'ia', 'novo', 'editar', 'deletar', 'salvar', 'cancelar'];
        for (const acao of order) {
            if (config[acao] && matches(e, config[acao])) {
                e.preventDefault();
                invokeAcao(acao);
                return;
            }
        }
    }

    function ensureHandler() {
        if (!dispatchHandler) {
            dispatchHandler = handleKey;
            document.addEventListener('keydown', handleKey, true);
        }
    }

    function removeHandler() {
        if (dispatchHandler) {
            document.removeEventListener('keydown', dispatchHandler, true);
            dispatchHandler = null;
        }
    }

    function tryFocus(el) {
        if (!el) return false;
        try { el.focus(); el.select?.(); return true; } catch { return false; }
    }

    function isVisible(el) {
        if (!el) return false;
        const rects = el.getClientRects();
        return !!(el.offsetWidth || el.offsetHeight || rects.length);
    }

    function focusSearchInput() {
        const inputSel = 'input[data-role="search-filter"], input#searchFilterInput, .search-container .search-input, input[type="search"]';
        let input = document.querySelector(inputSel);
        if (!input || !isVisible(input)) {
            const icon = document.querySelector('.search-container i.bi-search.action-icon');
            if (icon) {
                icon.dispatchEvent(new MouseEvent('click', { bubbles: true }));
                setTimeout(() => { tryFocus(document.querySelector(inputSel)); }, 50);
                return true;
            }
        }
        return tryFocus(input);
    }

    function stopCapture() {
        if (capture) {
            const c = capture;
            capture = null;
            c.input.classList.remove('is-capturing');
            c.input.removeEventListener('blur', c.blur);
            document.removeEventListener('keydown', c.key, true);
        }
        capturing = false;
    }

    function startCapture(inputId, dotnetRef, acaoKey) {
        stopCapture();
        const input = document.getElementById(inputId);
        if (!input) return;

        const onKey = function (e) {
            if (e.key === 'Escape') {
                e.preventDefault(); e.stopPropagation();
                stopCapture();
                return;
            }
            if (e.key === 'Control' || e.key === 'Shift' || e.key === 'Alt' || e.key === 'Meta') {
                e.preventDefault();
                return;
            }
            e.preventDefault(); e.stopPropagation();
            const combo = buildCombo(e);
            stopCapture();
            input.value = combo;
            try {
                const p = dotnetRef.invokeMethodAsync('SetAtalho', acaoKey, combo);
                if (p && typeof p.catch === 'function') p.catch(() => { /* no-op */ });
            } catch { /* no-op */ }
        };
        const onBlur = function () { stopCapture(); };

        capturing = true;
        capture = { input, key: onKey, blur: onBlur };
        input.classList.add('is-capturing');
        input.addEventListener('blur', onBlur);
        document.addEventListener('keydown', onKey, true);
        try { input.focus({ preventScroll: true }); } catch { input.focus(); }
        try { input.select(); } catch { /* no-op */ }
    }

    return {
        register: function (cfg) { if (cfg) Object.assign(config, cfg); },
        getConfig: function () { return Object.assign({}, config); },
        registerMain: function (dotnetRef) { mainRef = dotnetRef; ensureHandler(); },
        unregisterMain: function () { mainRef = null; removeHandler(); },
        registerPage: function (dotnetRef) { pageRef = dotnetRef; ensureHandler(); },
        clearPage: function () { pageRef = null; },
        focusSearchInput: function () { return focusSearchInput(); },
        startCapture: startCapture,
        stopCapture: stopCapture
    };
})();

window.BlazorInterop = window.BlazorInterop || {};

window.BlazorInterop.goBack = function (fallbackUrl) {
    try {
        if (history.length > 1 && document.referrer) {
            history.back();
        } else if (fallbackUrl) {
            window.location.href = fallbackUrl;
        }
    } catch {
        if (fallbackUrl) window.location.href = fallbackUrl;
    }
};

window.BlazorInterop.getClientRect = function (element) {
    if (!element || typeof element.getBoundingClientRect !== 'function') {
        return { left: 0, top: 0, width: 0, height: 0 };
    }
    const r = element.getBoundingClientRect();
    return { left: r.left, top: r.top, width: r.width, height: r.height };
};

// Namespace de compatibilidade (JTHotkeys) — delega ao appHotkeys
window.JTHotkeys = (function () {
    return {
        registerFilterHotkeys: function (dotnetRef) { window.appHotkeys.registerMain(dotnetRef); },
        unregisterFilterHotkeys: function () { window.appHotkeys.unregisterMain(); },
        enableFilterHotkeys: function () { /* compat: filtro sempre ativo */ },
        disableFilterHotkeys: function () { /* compat */ },
        enableSearchHotkeys: function () { /* compat: pesquisa sempre ativa */ },
        disableSearchHotkeys: function () { /* compat */ },
        focusSearchInput: function () { return window.appHotkeys.focusSearchInput(); },
        enableNewHotkeys: function (dotnetRef) { window.appHotkeys.registerPage(dotnetRef); },
        disableNewHotkeys: function () { window.appHotkeys.clearPage(); }
    };
})();