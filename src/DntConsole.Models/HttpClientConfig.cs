namespace DntConsole.Models;

public class HttpClientConfig
{
    public required string BaseAddress { set; get; }

    public required string UserAgent { set; get; }

    public required string Referrer { set; get; }
}