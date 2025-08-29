using FfkApi.Domain.Security.Criptografia;

namespace FfkApi.Infrastructure.Security.Criptografia;

public class EncriptadorBCrypt : IEncriptadorSenha
{
    public string Encriptar(string senha)
    {
        var senhaEncriptada = BCrypt.Net.BCrypt.EnhancedHashPassword(senha, 13);
        return senhaEncriptada;
    }

    public bool SenhaValida(string senha, string senhaEncriptada)
    {
        var senhaValida = false;
        try
        {
            senhaValida = BCrypt.Net.BCrypt.EnhancedVerify(senha, senhaEncriptada);
        }
        catch (Exception)
        {
            senhaValida = false;
        }

        return senhaValida;
    }

}
