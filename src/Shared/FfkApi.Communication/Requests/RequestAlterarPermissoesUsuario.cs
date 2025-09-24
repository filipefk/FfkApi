namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa uma requisição para alterar as permissões de um usuário.
/// </summary>
public class RequestAlterarPermissoesUsuario
{
    /// <summary>
    /// Obrigatório - Identificador único do usuário.
    /// </summary>
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Opcional - Lista de perfis de acesso que serão atribuídos ao usuário.
    /// </summary>
    public IList<string>? PerfisAcesso { get; set; } = null;

    /// <summary>
    /// Opcional - Lista de permissões específicas que serão atribuídas ao usuário.
    /// </summary>
    public IList<string>? Permissoes { get; set; } = null;
}