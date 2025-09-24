namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para alterar uma organização existente.
/// </summary>
public class RequestAlterarOrganizacao
{
    /// <summary>
    /// Obrigatório - Identificador único da organização.
    /// </summary>
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Nome da organização.
    /// </summary>
    public string? Nome { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Descrição da organização.
    /// </summary>
    public string? Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Opcional - E-mail do remetente para comunicações da organização.
    /// </summary>
    public string? RemetenteEmail { get; set; } = null;

    /// <summary>
    /// Opcional - Nome do remetente para comunicações da organização.
    /// </summary>
    public string? RemetenteNome { get; set; } = null;

    /// <summary>
    /// Opcional - Modelo de e-mail para ativação de conta.
    /// </summary>
    public string? ModeloEmailAtivacao { get; set; } = null;

    /// <summary>
    /// Opcional - Modelo de e-mail para redefinição de senha.
    /// </summary>
    public string? ModeloEmailNovaSenha { get; set; } = null;
}