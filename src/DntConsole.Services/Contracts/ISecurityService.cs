namespace DntConsole.Services.Contracts;

public interface ISecurityService
{
    string GetSha256Hash(string input);
}