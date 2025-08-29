namespace FfkApi.Domain.Security.Tokens;

public interface IValidadorTokenSistemaCliente
{
    public Guid ValidarEPegarIdSistemaCliente(string token);

    public Guid PegarIdSistemaCliente(string token);
}
