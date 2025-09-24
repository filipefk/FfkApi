namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para alterar uma equipe existente.
/// </summary>
public class RequestAlterarEquipe
{
    /// <summary>
    /// Obrigatório - Identificador único da equipe.
    /// </summary>
    public string? Id { get; set; } = string.Empty;
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
    /// Obrigatório - Lista de membros da equipe.
    /// </summary>
    public IList<RequestMembroEquipe>? Membros { get; set; } = [];

    /// <summary>
    /// Opcional - Organização associada à equipe. Se estiver vazio ou nulo não altera a Organização atual da Equipe
    /// </summary>
    public string? Organizacao { get; set; } = string.Empty;
}
