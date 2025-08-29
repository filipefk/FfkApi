using FfkApi.Communication.Responses;

namespace FfkApi.Application.IUseCases;

public interface ICadastrarEmLoteUseCase<TRequest, TRequests, TResponse>
{
    Task<ResponseCadastrarEmLote<TRequest, TResponse>> Execute(TRequests requests, CancellationToken cancellationToken);
}
