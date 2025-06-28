namespace JT_Finance.Shared.Services;

public class ToastSettings
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Heading { get; }
    public string Message { get; }
    public ToastLevel Level { get; }
    public int DurationInSeconds { get; } // 0 para não fechar automaticamente
    public DateTime Timestamp { get; }

    public ToastSettings(string heading, string message, ToastLevel level = ToastLevel.Info, int durationInSeconds = 5)
    {
        Heading = heading;
        Message = message;
        Level = level;
        DurationInSeconds = durationInSeconds;
        Timestamp = DateTime.Now;
    }
}

public enum ToastLevel
{
    Info,
    Success,
    Warning,
    Error
}
