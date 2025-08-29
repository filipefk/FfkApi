using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Token;

public interface IRefreshTokenUseCase
{
    Task<ResponseTokens> Execute(RequestNovoTokenUsuario request, CancellationToken cancellationToken);
}