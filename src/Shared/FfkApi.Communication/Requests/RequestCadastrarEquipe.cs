namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para cadastrar uma nova equipe.
/// </summary>
public class RequestCadastrarEquipe
{
    /// <summary>
    /// Obrigatório - Nome da equipe.
    /// </summary>
    public string? Nome { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Descrição da equipe.
    /// </summary>
    public string? Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Status da equipe. Aceita os valores "Ativo" ou "Inativo".
    /// </summary>
    public string? Status { get; set; } = string.Empty;

    /// <summary>
    /// Opcional - Lista de membros da equipe.
    /// </summary>
    public IList<RequestMembroEquipe>? Membros { get; set; } = [];

    /// <summary>
    /// Opcional - Organização associada à equipe. Se estiver vazio ou nulo será usada a Organização do usuário logado.
    /// </summary>
    public string? Organizacao { get; set; } = string.Empty;
}