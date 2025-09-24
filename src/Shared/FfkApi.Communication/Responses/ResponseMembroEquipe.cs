namespace FfkApi.Communication.Responses;

/// <summary>
/// Representa os dados de um membro de equipe.
/// </summary>
public class ResponseMembroEquipe
{
    /// <summary>
    /// Identificador único do membro da equipe.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador único do usuário associado ao membro.
    /// </summary>
    public Guid IdUsuario { get; set; }

    /// <summary>
    /// Nome do membro da equipe.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do membro da equipe.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o membro é líder da equipe.
    /// </summary>
    public bool Lider { get; set; } = false;
}