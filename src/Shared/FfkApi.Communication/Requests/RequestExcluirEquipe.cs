namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para exclusão de uma equipe.
/// </summary>
public class RequestExcluirEquipe
{
    /// <summary>
    /// Identificador único da equipe a ser excluída.
    /// </summary>
    public string? Id { get; set; } = string.Empty;
}