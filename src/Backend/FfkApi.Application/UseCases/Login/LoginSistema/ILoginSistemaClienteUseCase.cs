using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Login.LoginSistema;

public interface ILoginSistemaClienteUseCase
{
    Task<ResponseLoginSistemaCliente> Execute(RequestLoginSistemaCliente request, CancellationToken cancellationToken);
}
