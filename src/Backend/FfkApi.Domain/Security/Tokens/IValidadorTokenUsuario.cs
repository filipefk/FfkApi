namespace FfkApi.Domain.Security.Tokens;

public interface IValidadorTokenUsuario
{
    public Guid ValidarEPegarIdUsuario(string token);

    public Guid PegarIdUsuario(string token);
}
