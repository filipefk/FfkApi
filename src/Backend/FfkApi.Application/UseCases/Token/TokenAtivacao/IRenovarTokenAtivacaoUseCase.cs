using FfkApi.Communication.Requests;

namespace FfkApi.Application.UseCases.Token.TokenAtivacao;

public interface IRenovarTokenAtivacaoUseCase
{
    Task Execute(RequestRenovarTokenAtivacao request, CancellationToken cancellationToken);
}
