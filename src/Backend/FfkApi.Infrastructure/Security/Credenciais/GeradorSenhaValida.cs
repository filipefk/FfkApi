using FfkApi.Domain.Security.Credenciais;

namespace FfkApi.Infrastructure.Security.Credenciais;

public class GeradorSenhaValida : IGeradorSenhaValida
{
    public string GerarSenha(int tamanho = 10)
    {
        var randomizer = new Random();
        var password = "";

        if (tamanho == 0)
            return password;

        var upper = RandomString(1, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        password += upper;
        if (password.Length >= tamanho)
        {
            password = password.Substring(0, tamanho);
            return new string(password.OrderBy(_ => randomizer.Next()).ToArray());
        }

        var lower = RandomString(1, "abcdefghijklmnopqrstuvwxyz");
        password += lower;
        if (password.Length >= tamanho)
        {
            password = password.Substring(0, tamanho);
            return new string(password.OrderBy(_ => randomizer.Next()).ToArray());
        }

        var digit = RandomString(1, "0123456789");
        password += digit;
        if (password.Length >= tamanho)
        {
            password = password.Substring(0, tamanho);
            return new string(password.OrderBy(_ => randomizer.Next()).ToArray());
        }

        var symbol = RandomString(1, "!@#$%^&*()-_=+[]{}|;:,.<>?");
        password += symbol;
        if (password.Length >= tamanho)
        {
            password = password.Substring(0, tamanho);
            return new string(password.OrderBy(_ => randomizer.Next()).ToArray());
        }

        if (tamanho > password.Length)
        {
            var remaining = RandomString(tamanho - password.Length);
            password += remaining;
        }

        if (password.Length >= tamanho)
        {
            password = password.Substring(0, tamanho);
        }
        return new string(password.OrderBy(_ => randomizer.Next()).ToArray());
    }

    private static string RandomString(int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?")
    {
        var randomizer = new Random();

        if (length <= 0 || string.IsNullOrWhiteSpace(chars))
            return string.Empty;

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[randomizer.Next(0, chars.Length)];
        }

        return new string(result);
    }
}
