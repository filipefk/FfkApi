namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para cadastrar um novo usuário no sistema.
/// </summary>
public class RequestCadastrarUsuario
{
    /// <summary>
    /// Obrigatório - Nome completo do usuário.
    /// </summary>
    public string? Nome { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório - Endereço de e-mail do usuário. Não pode ser um e-mail já existente no sistema.
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
    /// Opcional - Nome da organização do usuário. Se não for informado, será usada a organização do usuário logado.
    /// </summary>
    public string? Organizacao { get; set; } = null;

    /// <summary>
    /// Opcional - Lista de perfis de acesso atribuídos ao usuário. Os perfis devem existir no sistema.
    /// </summary>
    public IList<string>? PerfisAcesso { get; set; } = null;

    /// <summary>
    /// Opcional - Lista de permissões atribuídas ao usuário. As permissões devem existir no sistema.
    /// </summary>
    public IList<string>? Permissoes { get; set; } = null;
}