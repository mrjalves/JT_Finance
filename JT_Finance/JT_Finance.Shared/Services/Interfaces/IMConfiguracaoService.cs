using JT_Finance.Shared.Models;

namespace JT_Finance.Shared.Services.Interfaces
{
    /// <summary>
    /// Contrato para o serviço de Configurações Financeiras, suportando API, 
    /// execução local e cache via JTServiceBase.
    /// </summary>
    public interface IMConfiguracaoService
    {
        // Métodos Padrão da Base
        Task<List<MConfiguracao>> GetAll();
        Task<MConfiguracao?> GetById(int id);
        Task<bool> Save(MConfiguracao configuracao);
        Task<int> SaveAsync(MConfiguracao configuracao);
        Task<bool> Delete(int id);

        // Método Específico
        /// <summary>
        /// Busca a configuração ativa do sistema. Cria uma nova caso não exista.
        /// </summary>
        Task<MConfiguracao> GetAtivaAsync();
    }
}