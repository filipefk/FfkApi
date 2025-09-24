namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para cadastrar uma nova organização.
/// </summary>
public class RequestCadastrarOrganizacao
{
    /// <summary>
    /// Obrigatório - Nome da organização. Deve ser único.
    /// </summary>
    public string? Nome { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Descrição da organização.
    /// </summary>
    public string? Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Opcional - E-mail do remetente para comunicações.
    /// </summary>
    public string? RemetenteEmail { get; set; } = null;

    /// <summary>
    /// Opcional - Nome do remetente para comunicações.
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