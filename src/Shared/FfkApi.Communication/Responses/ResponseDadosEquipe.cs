namespace FfkApi.Communication.Responses;

/// <summary>
/// Representa os dados detalhados de uma equipe.
/// </summary>
public class ResponseDadosEquipe
{
    /// <summary>
    /// Identificador único da equipe.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome da equipe.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da equipe.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Status atual da equipe.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Lista de membros que compõem a equipe.
    /// </summary>
    public IList<ResponseMembroEquipe> Membros { get; set; } = [];

    /// <summary>
    /// Nome da organização à qual a equipe pertence.
    /// </summary>
    public string Organizacao { get; set; } = string.Empty;
}