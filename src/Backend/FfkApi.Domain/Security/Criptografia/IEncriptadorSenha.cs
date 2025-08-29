namespace FfkApi.Domain.Security.Criptografia;

public interface IEncriptadorSenha
{
    string Encriptar(string senha);
    bool SenhaValida(string senha, string senhaEncriptada);
}
