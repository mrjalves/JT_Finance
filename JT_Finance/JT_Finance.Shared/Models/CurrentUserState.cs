using Microsoft.JSInterop;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace JT_Finance.Shared.Models
{
    /// <summary>
    /// Mantém o estado atual do usuário, gerencia a persistência (LocalStorage/Arquivo)
    /// e notifica a aplicação sobre mudanças. Registre como Singleton.
    /// </summary>
    public class CurrentUserState
    {
        private const string StorageKey = "jt_currentUser";
        private IJSRuntime? _jsRuntime;
        private bool _isInitialized = false;

        public string? UsuarioName { get; private set; }
        public string? UsuarioID { get; private set; }
        public string? Email { get; private set; }
        public bool Autenticado { get; private set; }

        public event Action? OnStateChanged;

        /// <summary>
        /// Inicializa o serviço com IJSRuntime (deve ser chamado uma vez no startup)
        /// </summary>
        public void Initialize(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        /// <summary>
        /// Tenta restaurar o estado do usuário a partir da persistência
        /// </summary>
        public async Task<bool> LoadStateAsync()
        {
            if (_jsRuntime == null)
                throw new InvalidOperationException("CurrentUserState não foi inicializado. Chame Initialize() primeiro.");

            try
            {
                string? json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var userDto = JsonSerializer.Deserialize<CurrentUserDto>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (userDto != null && userDto.Autenticado && !string.IsNullOrEmpty(userDto.UsuarioID))
                    {
                        UsuarioName = userDto.UsuarioName;
                        UsuarioID = userDto.UsuarioID;
                        Email = userDto.Email;
                        Autenticado = userDto.Autenticado;

                        _isInitialized = true;
                        NotifyStateChanged();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao carregar estado do usuário: {ex.Message}");
                // Em caso de erro, limpa o estado
                await ClearStateAsync();
            }

            _isInitialized = true;
            return false;
        }

        /// <summary>
        /// Define o usuário atual, persiste o estado e notifica os componentes
        /// </summary>
        public async Task SetUserAsync(FirebaseUser user)
        {
            if (_jsRuntime == null)
                throw new InvalidOperationException("CurrentUserState não foi inicializado. Chame Initialize() primeiro.");

            if (user == null)
            {
                await ClearUserAsync();
                return;
            }

            UsuarioName = user.DisplayName ?? user.Email;
            UsuarioID = user.Uid;
            Email = user.Email;
            Autenticado = !string.IsNullOrEmpty(user.Uid);

            await PersistStateAsync();
            NotifyStateChanged();
        }

        /// <summary>
        /// Limpa o usuário atual, remove a persistência e notifica os componentes
        /// </summary>
        public async Task ClearUserAsync()
        {
            UsuarioName = null;
            UsuarioID = null;
            Email = null;
            Autenticado = false;

            await RemovePersistenceAsync();
            NotifyStateChanged();
        }

        /// <summary>
        /// Verifica se o serviço está pronto para uso
        /// </summary>
        public bool IsReady => _isInitialized;

        private async Task PersistStateAsync()
        {
            try
            {
                var userDto = new CurrentUserDto
                {
                    UsuarioName = this.UsuarioName,
                    UsuarioID = this.UsuarioID,
                    Email = this.Email,
                    Autenticado = this.Autenticado
                };

                var json = JsonSerializer.Serialize(
                    userDto,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

                await _jsRuntime!.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao persistir estado do usuário: {ex.Message}");
            }
        }

        private async Task RemovePersistenceAsync()
        {
            try
            {
                await _jsRuntime!.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao remover persistência do usuário: {ex.Message}");
            }
        }

        /// <summary>
        /// Limpa apenas o estado local sem remover a persistência (para casos de erro)
        /// </summary>
        private async Task ClearStateAsync()
        {
            UsuarioName = null;
            UsuarioID = null;
            Email = null;
            Autenticado = false;

            // CORREÇÃO: Removeu as chamadas InvokeAsync e StateHasChanged
            // Apenas notifica os componentes inscritos
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnStateChanged?.Invoke();

        // DTO para serialização
        private class CurrentUserDto
        {
            public string? UsuarioName { get; set; }
            public string? UsuarioID { get; set; }
            public string? Email { get; set; }
            public bool Autenticado { get; set; }
        }
    }
}