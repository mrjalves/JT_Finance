using Microsoft.JSInterop;
using System.Threading.Tasks;
using JT_Finance.Shared.Models;
using System;

/// <summary>
/// Serviço de autenticação que integra com Firebase via JavaScript (JSInterop).
/// Encapsula as operações de login (e-mail/Google) e logout, e mantém o estado global
/// do usuário sincronizado através de <see cref="CurrentUserState"/> com persistência local.
/// </summary>
public class FirebaseAuthService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly CurrentUserState _currentUserState;
    private bool _isInitialized = false;

    public FirebaseAuthService(IJSRuntime jsRuntime, CurrentUserState currentUserState)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _currentUserState = currentUserState ?? throw new ArgumentNullException(nameof(currentUserState));
    }

    /// <summary>
    /// Inicializa o serviço e carrega o estado persistido do usuário
    /// </summary>
    public async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            // Inicializa o CurrentUserState se necessário
            if (!_currentUserState.IsReady)
            {
                _currentUserState.Initialize(_jsRuntime);
                await _currentUserState.LoadStateAsync();
            }
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Realiza login com email e senha
    /// </summary>
    public async Task<AuthResult> SignInWithEmailAsync(string email, string password)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("FirebaseAuthService não foi inicializado. Chame InitializeAsync() primeiro.");

        try
        {
            var result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.signInWithEmail", email, password);

            if (result.Success && result.User != null)
            {
                await _currentUserState.SetUserAsync(result.User);
            }

            return result;
        }
        catch (JSException jsEx)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erro de comunicação com Firebase: {jsEx.Message}"
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Realiza login com Google
    /// </summary>
    public async Task<AuthResult> SignInWithGoogleAsync()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("FirebaseAuthService não foi inicializado. Chame InitializeAsync() primeiro.");

        try
        {
            var result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.signInWithGoogle");

            if (result.Success && result.User != null)
            {
                await _currentUserState.SetUserAsync(result.User);
            }
            else if (result.Success && result.Redirect)
            {
                // Login com redirect iniciado - o estado será atualizado após o retorno
                return result;
            }

            return result;
        }
        catch (JSException jsEx)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erro de comunicação com Firebase: {jsEx.Message}"
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Verifica resultado de login por redirect (Google)
    /// </summary>
    public async Task<AuthResult> GetRedirectResultAsync()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("FirebaseAuthService não foi inicializado. Chame InitializeAsync() primeiro.");

        try
        {
            var result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.getGoogleRedirectResult");

            if (result.Success && result.User != null)
            {
                await _currentUserState.SetUserAsync(result.User);
            }

            return result;
        }
        catch (JSException jsEx)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erro ao verificar redirect: {jsEx.Message}"
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    public async Task<LogoutResult> SignOutAsync()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("FirebaseAuthService não foi inicializado. Chame InitializeAsync() primeiro.");

        try
        {
            await _jsRuntime.InvokeVoidAsync("FireBase.signOut");
            await _currentUserState.ClearUserAsync();

            return new LogoutResult { Success = true };
        }
        catch (JSException jsEx)
        {
            return new LogoutResult
            {
                Success = false,
                ErrorMessage = $"Erro ao fazer logout: {jsEx.Message}"
            };
        }
        catch (Exception ex)
        {
            return new LogoutResult
            {
                Success = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Verifica se o usuário está autenticado
    /// </summary>
    public bool IsUserAuthenticated => _currentUserState.Autenticado;

    /// <summary>
    /// Obtém o ID do usuário atual
    /// </summary>
    public string? CurrentUserId => _currentUserState.UsuarioID;

    /// <summary>
    /// Verifica se o serviço está pronto para uso
    /// </summary>
    public bool IsReady => _isInitialized;
}

// Modelos adicionais para melhor estruturação
public class AuthResult
{
    public bool Success { get; set; }
    public FirebaseUser? User { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Redirect { get; set; }
}

public class LogoutResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}