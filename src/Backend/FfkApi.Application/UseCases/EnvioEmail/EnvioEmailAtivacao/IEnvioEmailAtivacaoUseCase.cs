using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.EnvioEmail.EnvioEmailAtivacao;

public interface IEnvioEmailAtivacaoUseCase
{
    Task<IList<ResponseDadosUsuario>?> Execute(CancellationToken cancellationToken);
}
