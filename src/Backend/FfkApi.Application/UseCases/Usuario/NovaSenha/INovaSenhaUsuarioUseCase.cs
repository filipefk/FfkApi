using FfkApi.Communication.Requests;

namespace FfkApi.Application.UseCases.Usuario.NovaSenha;

public interface INovaSenhaUsuarioUseCase
{
    Task Execute(RequestNovaSenhaUsuario request, CancellationToken cancellationToken);
}
