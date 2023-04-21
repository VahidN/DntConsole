namespace DntConsole.Services;

public class AppHttpClientService
{
    private readonly HttpClient _httpClient;

    public AppHttpClientService(HttpClient httpClient) =>
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async Task<string> DownloadPageAsync(string path, Action<HttpRequestMessage>? configRequest = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        configRequest?.Invoke(request);
        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        await EnsureSuccessStatusCodeAsync(response);

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(responseStream);
        return await streamReader.ReadToEndAsync();
    }

    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = $"StatusCode: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";
        response.Content?.Dispose();
        throw new InvalidOperationException(content);
    }
}