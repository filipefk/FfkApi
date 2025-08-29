using FfkApi.Communication.Requests;

namespace FfkApi.Application.UseCases.Usuario.AlterarPermissoes;

public interface IAlterarPermissoesUsuarioUseCase
{
    Task Execute(RequestAlterarPermissoesUsuario request, CancellationToken cancellationToken);
}
