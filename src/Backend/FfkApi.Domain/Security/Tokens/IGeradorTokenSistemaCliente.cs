namespace FfkApi.Domain.Security.Tokens;

public interface IGeradorTokenSistemaCliente
{
    string Gerar(Guid idSistemaCliente);
}
