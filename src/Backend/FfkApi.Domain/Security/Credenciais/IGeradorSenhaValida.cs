namespace FfkApi.Domain.Security.Credenciais;

public interface IGeradorSenhaValida
{
    string GerarSenha(int tamanho = 10);
}
