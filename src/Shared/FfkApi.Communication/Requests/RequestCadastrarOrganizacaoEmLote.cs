namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para cadastrar múltiplas organizações em lote.
/// </summary>
public class RequestCadastrarOrganizacaoEmLote
{
    /// <summary>
    /// Lista de requisições para cadastro de organizações.
    /// </summary>
    public IList<RequestCadastrarOrganizacao> Organizacoes { get; set; } = [];
}