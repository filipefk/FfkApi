using FfkApi.Communication.Requests;

namespace FfkApi.Application.UseCases.Usuario.Ativar;

public interface IAtivarUsuarioUseCase
{
    Task Execute(RequestAtivarUsuario request, CancellationToken cancellationToken);
}
