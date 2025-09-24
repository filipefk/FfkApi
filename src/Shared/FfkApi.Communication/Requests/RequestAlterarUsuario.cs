namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para alterar os dados de um usuário.
/// </summary>
public class RequestAlterarUsuario
{
    /// <summary>
    /// Obrigatório - Identificador único do usuário.
    /// </summary>
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Nome completo do usuário.
    /// </summary>
    public string? Nome { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Endereço de e-mail do usuário. Deve ser um e-mail válido.
    /// </summary>
    public string? Email { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - CPF do usuário. Deve ser um CPF válido.
    /// </summary>
    public string? Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Opcional - Número de telefone do usuário. Se preenchido, deve ser um número de telefone válido.
    /// </summary>
    public string? Telefone { get; set; } = null;

    /// <summary>
    /// Obrigatório - Status atual do usuário. Ativo, Inativo, Ausente, Suspenso
    /// </summary>
    public string? Status { get; set; } = string.Empty;

    /// <summary>
    /// Opcional - Nome da organização do usuário. Se não for informado o valor atual não será alterado.
    /// </summary>
    public string? Organizacao { get; set; } = string.Empty;
}