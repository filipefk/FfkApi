namespace FfkApi.Communication.Responses;

/// <summary>
/// Representa os dados de uma organização retornados pela API.
/// </summary>
public class ResponseDadosOrganizacao
{
    /// <summary>
    /// Identificador único da organização.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome da organização.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da organização.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail do remetente utilizado pela organização.
    /// </summary>
    public string? RemetenteEmail { get; set; } = null;

    /// <summary>
    /// Nome do remetente utilizado pela organização.
    /// </summary>
    public string? RemetenteNome { get; set; } = null;

    /// <summary>
    /// Modelo de e-mail utilizado para ativação de conta.
    /// </summary>
    public string? ModeloEmailAtivacao { get; set; } = null;

    /// <summary>
    /// Modelo de e-mail utilizado para redefinição de senha.
    /// </summary>
    public string? ModeloEmailNovaSenha { get; set; } = null;
}