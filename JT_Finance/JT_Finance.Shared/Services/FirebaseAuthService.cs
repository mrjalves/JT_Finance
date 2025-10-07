using Microsoft.JSInterop;
using System.Threading.Tasks;
using JT_Finance.Shared.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

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
    private readonly HttpClient _httpClient;
    private const string FirebaseApiKey = "AIzaSyBMGBmvLhvar7rpys7NDvzEexbH_TZUEJs";

    // Configuração de domínios autorizados
    private readonly string[] _authorizedDomains = new[]
    {
        "localhost",
        "127.0.0.1",
        "jt-finance.web.app",
        "jt-finance.firebaseapp.com"
    };

    public FirebaseAuthService(IJSRuntime jsRuntime, CurrentUserState currentUserState)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _currentUserState = currentUserState ?? throw new ArgumentNullException(nameof(currentUserState));
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://identitytoolkit.googleapis.com/");
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

            // Verifica e configura o domínio atual
            await ConfigureAuthorizedDomainAsync();

            _isInitialized = true;
        }
    }

    /// <summary>
    /// Configura o domínio autorizado dinamicamente
    /// </summary>
    private async Task ConfigureAuthorizedDomainAsync()
    {
        try
        {
            // Obtém o host atual
            var currentHost = await GetCurrentHostAsync();

            // Verifica se o domínio atual está autorizado
            if (!IsDomainAuthorized(currentHost))
            {
                Console.WriteLine($"AVISO: Domínio '{currentHost}' não está na lista de autorizados. Usando fallback para localhost.");
            }

            // Configura estratégia de autenticação baseada no host
            await ConfigureAuthStrategyAsync(currentHost);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao configurar domínio: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém o host atual do aplicativo
    /// </summary>
    private async Task<string> GetCurrentHostAsync()
    {
        try
        {
#if ANDROID || WINDOWS
            return "localhost"; // Para apps desktop/mobile
#else
            // Para web, obtém do JavaScript
            return await _jsRuntime.InvokeAsync<string>("eval", "window.location.hostname");
#endif
        }
        catch
        {
            return "localhost"; // Fallback
        }
    }

    /// <summary>
    /// Verifica se o domínio está autorizado
    /// </summary>
    private bool IsDomainAuthorized(string domain)
    {
        foreach (var authorizedDomain in _authorizedDomains)
        {
            if (domain.Contains(authorizedDomain) || authorizedDomain.Contains(domain))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Configura a estratégia de autenticação baseada no host
    /// </summary>
    private async Task ConfigureAuthStrategyAsync(string host)
    {
        try
        {
            // Para localhost ou domínios não autorizados, usa estratégia alternativa
            if (host == "localhost" || host == "127.0.0.1" || !IsDomainAuthorized(host))
            {
                await _jsRuntime.InvokeVoidAsync("eval",
                    @"
                    if (typeof window.firebaseAuthStrategy === 'undefined') {
                        window.firebaseAuthStrategy = 'rest-api';
                    }
                    ");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao configurar estratégia de auth: {ex.Message}");
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
#if ANDROID || WINDOWS
            // 🔹 Login via REST API no modo desktop/mobile
            return await SignInWithEmailRestAsync(email, password);
#else
            // Tenta primeiro via JSInterop, se falhar usa REST API como fallback
            try
            {
                var result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.signInWithEmail", email, password);
                if (result.Success && result.User != null)
                    await _currentUserState.SetUserAsync(result.User);
                return result;
            }
            catch (JSException jsEx) when (jsEx.Message.Contains("unauthorized-domain"))
            {
                // Fallback para REST API em caso de domínio não autorizado
                Console.WriteLine("Domínio não autorizado no JSInterop, usando REST API como fallback...");
                return await SignInWithEmailRestAsync(email, password);
            }
#endif
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
#if ANDROID || WINDOWS
            // 🔹 Para desktop/mobile, usa autenticação nativa ou desabilita Google Auth
            return await SignInWithGoogleNativeAsync();
#else
            // Para web, tenta diferentes estratégias
            return await SignInWithGoogleWebAsync();
#endif
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
    /// Estratégia de login com Google para web
    /// </summary>
    private async Task<AuthResult> SignInWithGoogleWebAsync()
    {
        try
        {
            // Primeiro tenta com popup (menos problemas de domínio)
            var result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.signInWithGooglePopup");

            if (result.Success && result.User != null)
            {
                await _currentUserState.SetUserAsync(result.User);
                return result;
            }

            // Se popup falhar, tenta com redirect
            if (!result.Success)
            {
                result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.signInWithGoogleRedirect");

                if (result.Success && result.Redirect)
                {
                    return result; // Redirect iniciado
                }
            }

            return result;
        }
        catch (JSException jsEx) when (jsEx.Message.Contains("unauthorized-domain"))
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Domínio não autorizado no Firebase. Configure localhost no Firebase Console."
            };
        }
    }

    /// <summary>
    /// Estratégia de login com Google para native apps
    /// </summary>
    private async Task<AuthResult> SignInWithGoogleNativeAsync()
    {
        try
        {
            // Primeiro tenta com popup (menos problemas de domínio)
            var result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.signInWithGooglePopup");

            if (result.Success && result.User != null)
            {
                await _currentUserState.SetUserAsync(result.User);
                return result;
            }

            // Se popup falhar, tenta com redirect
            if (!result.Success)
            {
                result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.signInWithGoogleRedirect");

                if (result.Success && result.Redirect)
                {
                    return result; // Redirect iniciado
                }
            }

            return result;
        }
        catch (JSException jsEx) when (jsEx.Message.Contains("unauthorized-domain"))
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Domínio não autorizado no Firebase. Configure localhost no Firebase Console."
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
#if ANDROID || WINDOWS
            return new AuthResult { Success = false, ErrorMessage = "Redirect não suportado em apps desktop/mobile." };
#else
            var result = await _jsRuntime.InvokeAsync<AuthResult>("FireBase.getGoogleRedirectResult");
            if (result.Success && result.User != null)
                await _currentUserState.SetUserAsync(result.User);
            return result;
#endif
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
#if ANDROID || WINDOWS
            await _currentUserState.ClearUserAsync();
#else
            await _jsRuntime.InvokeVoidAsync("FireBase.signOut");
            await _currentUserState.ClearUserAsync();
#endif

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
    /// Login com email/senha via REST API (fallback para domínios não autorizados)
    /// </summary>
    private async Task<AuthResult> SignInWithEmailRestAsync(string email, string password)
    {
        try
        {
            var endpoint = $"/v1/accounts:signInWithPassword?key={FirebaseApiKey}";
            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(endpoint, payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMessage = ExtractErrorMessage(errorContent);
                return new AuthResult { Success = false, ErrorMessage = $"Erro Firebase: {errorMessage}" };
            }

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();

            // Converte a resposta REST para FirebaseUser
            var user = new FirebaseUser
            {
                Uid = content.GetProperty("localId").GetString(),
                Email = content.GetProperty("email").GetString(),
                IdToken = content.GetProperty("idToken").GetString(),
                LocalId = content.GetProperty("localId").GetString()
            };

            await _currentUserState.SetUserAsync(user);

            return new AuthResult
            {
                Success = true,
                User = user
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erro na autenticação REST: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Extrai mensagem de erro do response do Firebase
    /// </summary>
    private string ExtractErrorMessage(string errorContent)
    {
        try
        {
            var errorJson = JsonDocument.Parse(errorContent);
            if (errorJson.RootElement.TryGetProperty("error", out var errorObj))
            {
                if (errorObj.TryGetProperty("message", out var message))
                {
                    return message.GetString() ?? "Erro desconhecido";
                }
            }
        }
        catch
        {
            // Ignora erro de parsing e retorna o conteúdo original
        }

        return errorContent;
    }

    /// <summary>
    /// Verifica se o usuário está autenticado
    /// </summary>
    public bool IsUserAuthenticated => _currentUserState.Autenticado;

    /// <summary>
    /// Obtém o ID do usuário atual
    /// </summary>
    public string CurrentUserId => _currentUserState.UsuarioID;

    /// <summary>
    /// Verifica se o serviço está pronto para uso
    /// </summary>
    public bool IsReady => _isInitialized;
}

/// <summary>
/// Resultado do logout
/// </summary>
public class LogoutResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}