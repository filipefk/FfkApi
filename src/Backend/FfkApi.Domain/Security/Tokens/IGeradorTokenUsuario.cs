namespace FfkApi.Domain.Security.Tokens;

public interface IGeradorTokenUsuario
{
    string Gerar(Guid idUsuario);
}
