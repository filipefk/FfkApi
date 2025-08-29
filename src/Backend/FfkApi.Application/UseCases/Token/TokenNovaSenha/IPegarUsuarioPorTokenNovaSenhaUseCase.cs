using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Token.TokenNovaSenha;

public interface IPegarUsuarioPorTokenNovaSenhaUseCase
{
    Task<ResponseNomeUsuario> Execute(RequestPegarUsuarioPorTokenNovaSenha request, CancellationToken cancellationToken);
}
