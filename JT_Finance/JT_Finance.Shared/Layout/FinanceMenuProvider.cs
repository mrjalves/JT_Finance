using JT_Aplicacao.Classes;
using JT_Aplicacao.Interface;
using JT_UserInterface.Shared.Services.Interfaces.Comum;

namespace JT_Finance.Shared.Layout
{
    /// <summary>
    /// Provider do menu lateral para o módulo JT_Finance.
    /// Implementa IModuloMenuProvider para exibir itens agrupados por módulo.
    /// Implementa também IMenuProvider para retrocompatibilidade.
    /// </summary>
    public class FinanceMenuProvider : IModuloMenuProvider, IMenuProvider
    {
        // ── IModuloMenuProvider ────────────────────────────────────
        public List<SModuloMenu> GetModulos()
        {
            return new List<SModuloMenu>
            {
                new SModuloMenu
                {
                    NomeModulo = "Financeiro",
                    Icone      = "bi-cash-stack",
                    Modulo     = eModuloSistema.Financeiro,
                    Itens      = new List<SItemMenu>
                    {
                        new SItemMenu { Url = "",                  Texto = "Home",         Icone = "bi-house-door-fill",  MatchAll = true, Titulo = "Home" },
                        new SItemMenu { Url = "historico",         Texto = "Histórico",    Icone = "bi-calendar-fill",    MatchAll = true, Titulo = "Histórico" },
                        new SItemMenu { Url = "receitas",          Texto = "Receitas",     Icone = "bi-graph-up-arrow",   Titulo = "Receitas" },
                        new SItemMenu { Url = "despesas",          Texto = "Despesas",     Icone = "bi-graph-down-arrow", Titulo = "Despesas" },
                        new SItemMenu { Url = "cartoes-credito",   Texto = "Cartões",      Icone = "bi-credit-card-fill", Titulo = "Cartões de Crédito" },
                    }
                },
                new SModuloMenu
                {
                    NomeModulo = "Gestão",
                    Icone      = "bi-sliders",
                    Modulo     = eModuloSistema.Gestao,
                    Itens      = new List<SItemMenu>
                    {
                        new SItemMenu { Url = "gerenciar-categorias", Texto = "Categorias", Icone = "bi-tags-fill", Titulo = "Gerenciar Categorias" },
                    }
                }
                
            };
        }

        // ── IMenuProvider (retrocompatibilidade) ───────────────────
        public List<SItemMenu> GetMenu()
            => GetModulos().SelectMany(m => m.Itens).ToList();
    }
}