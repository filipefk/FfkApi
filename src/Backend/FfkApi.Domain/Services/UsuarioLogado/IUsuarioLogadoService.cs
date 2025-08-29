using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Services.UsuarioLogado;

public interface IUsuarioLogadoService
{
    Task<Usuario> PegarUsuarioLogadoAtivo(CancellationToken cancellationToken);

    Task<Usuario?> PegarUsuarioDoTokenEnviado(CancellationToken cancellationToken);
}
