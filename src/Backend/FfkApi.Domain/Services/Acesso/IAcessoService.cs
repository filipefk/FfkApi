namespace FfkApi.Domain.Services.Acesso;

public interface IAcessoService
{
    Task AplicarPerfisAoUsuarioAsync(Guid idUsuario, IList<Guid> perfilIds);
    Task<bool> UsuarioTemPermissaoAsync(Guid idUsuario, string nomePermissao);
    Task<bool> UsuarioAdministradorAsync(Guid idUsuario);
    Task AtualizarUsuariosDoPerfilAsync(Guid idPerfil, bool aplicarAlteracoes);
}

