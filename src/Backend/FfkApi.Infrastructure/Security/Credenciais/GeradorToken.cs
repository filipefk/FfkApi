using FfkApi.Domain.Security.Credenciais;
using System.Security.Cryptography;

namespace FfkApi.Infrastructure.Security.Credenciais;

public class GeradorToken : IGeradorToken
{
    public string GerarToken(int tamanhoEmBytes = 32)
    {
        var bytes = new byte[tamanhoEmBytes];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        var base64 = Convert.ToBase64String(bytes);

        var urlSafeBase64 = base64.Replace("+", "-").Replace("/", "_").TrimEnd('=');

        return urlSafeBase64;
    }
}
