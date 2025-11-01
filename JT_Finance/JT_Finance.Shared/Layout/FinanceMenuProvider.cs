using JT_Aplicacao;

using JT_UserInterface.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace JT_Finance.Shared.Layout
{
    public class FinanceMenuProvider : IMenuProvider
    {
        public List<SItemMenu> GetMenu() => MenuLateral.ItensMenu;
    }
}