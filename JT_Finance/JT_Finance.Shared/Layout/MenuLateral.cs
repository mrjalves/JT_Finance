using JT_Aplicacao.UserInterface;

using JT_UserInterface.Shared.Layout;

namespace JT_Finance.Shared.Layout
{
    /// <summary>
    /// Mantém o MenuLateral estático para uso interno legacy.
    /// Delega para a implementação do FinanceMenuProvider.
    /// </summary>
    public static class MenuLateral
    {
        public static List<SItemMenu> ItensMenu { get; } =
            new FinanceMenuProvider().GetMenu();
    }
}
