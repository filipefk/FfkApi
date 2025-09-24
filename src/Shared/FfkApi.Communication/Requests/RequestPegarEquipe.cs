namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para obter os dados de uma equipe.
/// </summary>
public class RequestPegarEquipe
{
    /// <summary>
    /// Identificador único da equipe a ser consultada.
    /// </summary>
    public string? Id { get; set; } = string.Empty;
}