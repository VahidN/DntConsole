namespace DntConsole.Models;

public class AppConfig
{
    public required ConnectionStrings ConnectionStrings { set; get; }

    public required HttpClientConfig HttpClientConfig { set; get; }
}