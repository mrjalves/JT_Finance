using System;
using System.Collections.Generic;
using JT_Aplicacao.Classes;
using JT_Classes.Financeiro;
using JT_UserInterface.Shared.Services.Interfaces.Comum;

namespace JT_Finance.Shared.Layout
{
    public class FinanceiroRegrasProvider : IModuloRegrasProvider
    {
        public string NomeModulo => "Financeiro";
        public eModuloSistema Modulo => eModuloSistema.Financeiro;

        public List<EntidadeRegrasInfo> GetEntidades()
        {
            return new List<EntidadeRegrasInfo>
            {
                new EntidadeRegrasInfo("Despesa", "Gestão de despesas e contas a pagar", typeof(Despesa)),
                new EntidadeRegrasInfo("Receita", "Gestão de receitas e contas a receber", typeof(Receita))
            };
        }
    }
}
