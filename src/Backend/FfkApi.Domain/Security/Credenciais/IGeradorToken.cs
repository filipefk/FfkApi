namespace FfkApi.Domain.Security.Credenciais;

public interface IGeradorToken
{
    string GerarToken(int tamanhoEmBytes = 32);
}
