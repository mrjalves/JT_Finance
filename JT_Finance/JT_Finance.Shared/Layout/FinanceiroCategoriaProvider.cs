using System;
using System.Collections.Generic;
using JT_Aplicacao.Classes;
using JT_Classes.Financeiro;
using JT_UserInterface.Shared.Services.Interfaces.Comum;

namespace JT_Finance.Shared.Layout
{
    public class FinanceiroCategoriaProvider : IModuloCategoriaProvider
    {
        public string NomeModulo => "Financeiro";
        public eModuloSistema Modulo => eModuloSistema.Financeiro;

        public List<Type> GetEntidadesComCategoria()
        {
            return new List<Type>
            {
                typeof(Despesa),
                typeof(Receita)
            };
        }
    }
}
