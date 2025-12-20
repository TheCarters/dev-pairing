namespace DevPairing.Api.Services;

public interface INtfyService
{
    Task SendNotificationAsync(string topic, string title, string message, string? clickUrl = null);
}
