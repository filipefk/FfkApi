namespace FfkApi.Communication.Responses;

/// <summary>
/// Representa os dados de um usuário retornados pela API.
/// </summary>
public class ResponseDadosUsuario
{
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail do usuário.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// CPF do usuário.
    /// </summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Número de telefone do usuário.
    /// </summary>
    public string? Telefone { get; set; } = null;

    /// <summary>
    /// Status atual do usuário (ex: Ativo, Inativo).
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Organização associada ao usuário.
    /// </summary>
    public string Organizacao { get; set; } = string.Empty;

    /// <summary>
    /// Lista de perfis de acesso atribuídos ao usuário.
    /// </summary>
    public IList<string> PerfisAcesso { get; set; } = [];

    /// <summary>
    /// Lista de permissões atribuídas ao usuário.
    /// </summary>
    public IList<string> Permissoes { get; set; } = [];
}