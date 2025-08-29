using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Token.TokenNovaSenha;

public interface INovoTokenNovaSenhaUseCase
{
    Task<ResponseNomeUsuario> Execute(RequestNovoTokenNovaSenha request, CancellationToken cancellationToken);
}
