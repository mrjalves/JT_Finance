using JT_Aplicacao;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Layout
{
    public static class MenuLateral
    {
        // Corrigido: propriedade somente leitura com getter correto        
        public static List<SItemMenu> ItensMenu { get; } = GetItensMenu();


        private static List<SItemMenu> GetItensMenu()
        {

            return new List<SItemMenu>
            {
                new SItemMenu { Url = "", Texto = "Home", Icone = "bi-house-door-fill", MatchAll = true, Titulo = "Home" },
                new SItemMenu { Url = "historico", Texto = "Histórico", Icone = "bi-calendar-fill", MatchAll = true, Titulo = "Histórico" },
                new SItemMenu { Url = "receitas", Texto = "Receitas", Icone = "bi-graph-up-arrow", Titulo = "Receitas" },
                new SItemMenu { Url = "despesas", Texto = "Despesas", Icone = "bi-graph-down-arrow", Titulo = "Despesas" },
                new SItemMenu { Url = "cartoes-credito", Texto = "Cartões", Icone = "bi-credit-card-fill", Titulo = "Cartões de Crédito" },
                new SItemMenu { Url = "listacompras", Texto = "Lista Compras", Icone = "bi-cart-fill", Titulo = "Lista de Compras" },
                new SItemMenu { Url = "gerenciar-categorias", Texto = "Categorias", Icone = "bi-tags-fill", Titulo = "Gerenciar Categorias" }
            };
        }


    }
}
