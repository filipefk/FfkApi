namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa um membro de uma equipe na requisição de cadastro.
/// </summary>
public class RequestMembroEquipe
{
    /// <summary>
    /// Obrigatório - E-mail do membro da equipe. Deve ser um e-mail válido de um usuário ativo no sistema
    /// </summary>
    public string? Email { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Indica se o membro é ou não o líder da equipe.
    /// </summary>
    public bool? Lider { get; set; } = false;
}