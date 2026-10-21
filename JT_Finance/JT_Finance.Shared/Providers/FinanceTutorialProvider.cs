using System.Collections.Generic;
using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services.Interfaces.Comum;

namespace JT_Finance.Shared.Providers
{
    /// <summary>
    /// Provedor oficial de tutoriais interativos guiados para as páginas do JT_Finance.
    /// Registrado via <see cref="ITutorialProvider"/> e gerenciado pelo TutorialService / TutorialHost.
    /// Permite exibição automática na primeira visita e reabertura manual pelo botão (?) no cabeçalho.
    /// </summary>
    public class FinanceTutorialProvider : ITutorialProvider
    {
        public List<TutorialDefinicao> ObterTutoriais() => new()
        {
            // ──────────────────────────────────────────────
            // 1. Dashboard Executivo Financeiro
            // ──────────────────────────────────────────────
            new TutorialDefinicao
            {
                Chave = "finance-dashboard",
                Rotas = new List<string> { "/", "/home" },
                Titulo = "Dashboard Executivo Financeiro",
                Descricao = "Visão panorâmica e consolidada da saúde financeira, fluxo de caixa, vencimentos e inteligência analítica.",
                Icone = "bi-wallet2",
                MostrarUmaVez = true,
                Passos = new List<TutorialPasso>
                {
                    new TutorialPasso
                    {
                        Titulo = "Navegação Mensal & Controles Rápidos",
                        Descricao = "Navegue entre meses passados e futuros com as setas, retorne ao mês corrente com o botão 'Mês Atual' e atualize as métricas a qualquer momento.",
                        Seletor = "[data-tutorial='dashboard-header']",
                        Posicao = "bottom"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Receitas do Mês",
                        Descricao = "Consolidação de todas as entradas, salários, recebimentos e faturamentos confirmados no mês de referência selecionado.",
                        Seletor = "[data-tutorial='kpi-receitas']",
                        Posicao = "bottom"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Despesas do Mês",
                        Descricao = "Total de saídas registradas no período. Clique diretamente no card para abrir o modal de auditoria com todas as despesas detalhadas.",
                        Seletor = "[data-tutorial='kpi-despesas']",
                        Posicao = "bottom"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Saldo Líquido Operacional",
                        Descricao = "Diferença real entre receitas e despesas. Destaca superávit em tom azul/verde ou déficit em vermelho para controle orçamentário.",
                        Seletor = "[data-tutorial='kpi-saldo']",
                        Posicao = "bottom"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Saúde Orçamentária & Comprometimento",
                        Descricao = "Mede quanto da sua receita está comprometida com gastos. A barra de progresso visual avisa se o orçamento está saudável, em atenção ou excedido.",
                        Seletor = "[data-tutorial='kpi-saude']",
                        Posicao = "bottom"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Próximas ao Vencimento",
                        Descricao = "Monitore compromissos e contas que vencem nos próximos dias para evitar multas, juros e manter o planejamento em dia. Clique para inspecionar.",
                        Seletor = "[data-tutorial='kpi-a-vencer']",
                        Posicao = "bottom"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Contas Vencidas (Atenção Urgente)",
                        Descricao = "Alerta crítico de despesas cujo prazo de pagamento já expirou. Clique no card para ver a lista e regularizar pendências.",
                        Seletor = "[data-tutorial='kpi-vencidas']",
                        Posicao = "bottom"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Acessos Rápidos Operacionais",
                        Descricao = "Atalhos imediatos para cadastrar Despesas, Receitas, gerenciar Cartões de Crédito ou compor novos Dashboards com Inteligência Artificial.",
                        Seletor = "[data-tutorial='dashboard-atalhos']",
                        Posicao = "top"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Últimas Despesas Registradas",
                        Descricao = "Feed dinâmico dos lançamentos mais recentes do mês, com categoria, data, valor e indicação se a conta já foi paga ou continua pendente.",
                        Seletor = "[data-tutorial='dashboard-ultimas-despesas']",
                        Posicao = "top"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Contas Vencidas Prioritárias",
                        Descricao = "Lista rápida das contas atrasadas com indicador de quantos dias estão vencidas e botão de auditoria imediata.",
                        Seletor = "[data-tutorial='dashboard-contas-criticas']",
                        Posicao = "top"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Despesas por Categoria",
                        Descricao = "Gráfico interativo em rosca exibindo a distribuição percentual dos seus gastos por categoria (moradia, alimentação, transporte, etc.).",
                        Seletor = "[data-tutorial='grafico-categorias']",
                        Posicao = "top"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Despesas por Tipo",
                        Descricao = "Gráfico de barras comparando despesas fixas versus despesas variáveis para apoiar decisões estratégicas de redução de custos.",
                        Seletor = "[data-tutorial='grafico-tipos']",
                        Posicao = "top"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Consultor Financeiro & Assistente IA",
                        Descricao = "Clique no ícone de chat no topo a qualquer momento para conversar com o Assistente IA sobre metas de economia e diagnóstico financeiro.",
                        Seletor = "header .header-actions button[title='Chat IA']",
                        Posicao = "bottom"
                    },
                    new TutorialPasso
                    {
                        Titulo = "Alternar Tema Claro / Escuro",
                        Descricao = "Alterne o tema visual entre claro e escuro para melhor conforto visual em qualquer ambiente.",
                        Seletor = "header .header-actions .theme-toggle",
                        Posicao = "bottom"
                    }
                }
            },

            // ──────────────────────────────────────────────
            // 2. Gerenciamento de Despesas
            // ──────────────────────────────────────────────
            new TutorialDefinicao
            {
                Chave = "finance-despesas",
                Rotas = new List<string> { "/despesas" },
                Titulo = "Gestão de Despesas & Contas a Pagar",
                Descricao = "Controle completo de contas a pagar, datas de vencimento, comprovantes e baixa de pagamentos.",
                Icone = "bi-cash-coin",
                MostrarUmaVez = true,
                Passos = new List<TutorialPasso>
                {
                    new TutorialPasso
                    {
                        Titulo = "Listagem e Filtros de Despesas",
                        Descricao = "Consulte todas as despesas cadastradas, filtre por período, status de pagamento ou categoria.",
                        Seletor = ".card",
                        Posicao = "bottom"
                    }
                }
            },

            // ──────────────────────────────────────────────
            // 3. Gerenciamento de Receitas
            // ──────────────────────────────────────────────
            new TutorialDefinicao
            {
                Chave = "finance-receitas",
                Rotas = new List<string> { "/receitas" },
                Titulo = "Gestão de Receitas & Entradas",
                Descricao = "Acompanhamento de todas as fontes de receitas, entradas financeiras e rendimentos periódicos.",
                Icone = "bi-plus-circle",
                MostrarUmaVez = true,
                Passos = new List<TutorialPasso>
                {
                    new TutorialPasso
                    {
                        Titulo = "Entradas Financeiras",
                        Descricao = "Cadastre novas fontes de renda, acompanhe valores recebidos e consulte o histórico de lançamentos.",
                        Seletor = ".card",
                        Posicao = "bottom"
                    }
                }
            },

            // ──────────────────────────────────────────────
            // 4. Cartões de Crédito & Faturas
            // ──────────────────────────────────────────────
            new TutorialDefinicao
            {
                Chave = "finance-cartoes",
                Rotas = new List<string> { "/cartoes-credito" },
                Titulo = "Cartões de Crédito & Faturas",
                Descricao = "Gestão centralizada de cartões de crédito, limites disponíveis, parcelamentos e fechamento de faturas.",
                Icone = "bi-credit-card-2-front",
                MostrarUmaVez = true,
                Passos = new List<TutorialPasso>
                {
                    new TutorialPasso
                    {
                        Titulo = "Cartões Cadastrados",
                        Descricao = "Visualize limites totais e disponíveis, melhores dias de compra e datas de vencimento de cada cartão.",
                        Seletor = ".card",
                        Posicao = "bottom"
                    }
                }
            },

            // ──────────────────────────────────────────────
            // 5. Dashboards & Composição IA
            // ──────────────────────────────────────────────
            new TutorialDefinicao
            {
                Chave = "finance-dashboards",
                Rotas = new List<string> { "/dashboards" },
                Titulo = "Dashboards Personalizados & EDW",
                Descricao = "Criação de novos painéis com Inteligência Artificial e catálogo de indicadores financeiros EDW.",
                Icone = "bi-speedometer2",
                MostrarUmaVez = true,
                Passos = new List<TutorialPasso>
                {
                    new TutorialPasso
                    {
                        Titulo = "Composição com IA",
                        Descricao = "Utilize o botão 'Compor dashboard com IA' para descrever em linguagem natural o painel que deseja gerar.",
                        Seletor = "button:has(.bi-stars)",
                        Posicao = "bottom"
                    }
                }
            }
        };
    }
}
