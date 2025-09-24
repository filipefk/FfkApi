namespace FfkApi.Communication.Responses;

/// <summary>
/// Representa uma resposta de erro retornada pela API.
/// </summary>
public class ResponseErro
{
    /// <summary>
    /// Lista de mensagens de erro detalhando os problemas encontrados.
    /// </summary>
    public IList<string> MensagensDeErro { get; set; }

    /// <summary>
    /// Indica se o token de autenticação está expirado.
    /// </summary>
    public bool TokenEstaExpirado { get; set; }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ResponseErro"/> com uma lista de mensagens de erro.
    /// </summary>
    /// <param name="mensagensDeErro">Lista de mensagens de erro.</param>
    public ResponseErro(IList<string> mensagensDeErro) => MensagensDeErro = mensagensDeErro;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ResponseErro"/> com uma única mensagem de erro.
    /// </summary>
    /// <param name="mensagemDeErro">Mensagem de erro.</param>
    public ResponseErro(string mensagemDeErro) => MensagensDeErro = [mensagemDeErro];
}