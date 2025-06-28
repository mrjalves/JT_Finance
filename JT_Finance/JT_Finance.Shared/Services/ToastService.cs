namespace JT_Finance.Shared.Services;

public class ToastService : IDisposable
{
    public event Action<ToastSettings> OnShowToast;
    public event Action<Guid> OnHideToast; // Para remoção explícita se necessário
    public event Action OnClearToasts;     // Para limpar todos os toasts

    private List<ToastSettings> _activeToasts = new List<ToastSettings>();
    public IReadOnlyList<ToastSettings> ActiveToasts => _activeToasts.AsReadOnly();

    public void ShowToast(string heading, string message, ToastLevel level = ToastLevel.Info, int durationInSeconds = 5)
    {
        var settings = new ToastSettings(heading, message, level, durationInSeconds);
        _activeToasts.Add(settings);
        OnShowToast?.Invoke(settings);
    }

    public void ShowSuccess(string heading, string message, int durationInSeconds = 5)
    {
        ShowToast(heading, message, ToastLevel.Success, durationInSeconds);
    }

    public void ShowError(string heading, string message, int durationInSeconds = 7) // Erros geralmente ficam um pouco mais
    {
        ShowToast(heading, message, ToastLevel.Error, durationInSeconds);
    }

    public void ShowInfo(string heading, string message, int durationInSeconds = 5)
    {
        ShowToast(heading, message, ToastLevel.Info, durationInSeconds);
    }

    public void ShowWarning(string heading, string message, int durationInSeconds = 7)
    {
        ShowToast(heading, message, ToastLevel.Warning, durationInSeconds);
    }

    public void RemoveToast(Guid id)
    {
        var toastToRemove = _activeToasts.FirstOrDefault(t => t.Id == id);
        if (toastToRemove != null)
        {
            _activeToasts.Remove(toastToRemove);
            OnHideToast?.Invoke(id); // Notifica o container para remover o toast específico da UI
        }
    }

    public void ClearAll()
    {
        _activeToasts.Clear();
        OnClearToasts?.Invoke();
    }

    public void Dispose()
    {
        // Limpar eventos se necessário para evitar memory leaks em cenários mais complexos,
        // mas para Blazor WASM/Server com DI Scoped/Singleton, geralmente não é um grande problema.
        OnShowToast = null;
        OnHideToast = null;
        OnClearToasts = null;
    }
}
