using Microsoft.Extensions.Options;

namespace DevPairing.Api.Services;

public class NtfyService : INtfyService
{
    private readonly HttpClient _httpClient;
    private readonly NtfyOptions _options;

    public NtfyService(HttpClient httpClient, IOptions<NtfyOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendNotificationAsync(string topic, string title, string message, string? clickUrl = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/{topic}")
        {
            Content = new StringContent(message)
        };

        if (!string.IsNullOrEmpty(title))
        {
            request.Headers.Add("Title", title);
        }

        if (!string.IsNullOrEmpty(clickUrl))
        {
            request.Headers.Add("Click", clickUrl);
        }

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Log error but don't crash app
            Console.WriteLine($"Error sending ntfy notification: {ex.Message}");
        }
    }
}
