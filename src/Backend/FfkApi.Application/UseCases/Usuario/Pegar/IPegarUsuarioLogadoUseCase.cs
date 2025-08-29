using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Usuario.Pegar;

public interface IPegarUsuarioLogadoUseCase
{

    Task<ResponseDadosUsuario> Execute(CancellationToken cancellationToken);
}