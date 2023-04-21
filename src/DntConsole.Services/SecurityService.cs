using System.Text;
using DntConsole.Services.Contracts;

namespace DntConsole.Services;

public class SecurityService : ISecurityService
{
    public string GetSha256Hash(string input)
    {
        var byteValue = Encoding.UTF8.GetBytes(input);
        var byteHash = SHA256.HashData(byteValue);
        return Encoding.UTF8.GetString(byteHash);
    }
}