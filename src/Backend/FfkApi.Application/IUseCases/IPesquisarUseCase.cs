using FfkApi.Communication.Responses;
using Microsoft.AspNetCore.Http;

namespace FfkApi.Application.IUseCases;

public interface IPesquisarUseCase<TResponse>
{
    Task<ResponsePaginado<TResponse>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken);
}
