using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Login.LoginUsuario;

public interface ILoginUsuarioUseCase
{
    Task<ResponseLoginUsuario> Execute(RequestLoginUsuario request, CancellationToken cancellationToken);
}