using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Token.TokenAtivacao;

public interface IPegarUsuarioPorTokenAtivacaoUseCase
{
    Task<ResponseNomeUsuario> Execute(RequestPegarUsuarioPorTokenAtivacao request, CancellationToken cancellationToken);
}
