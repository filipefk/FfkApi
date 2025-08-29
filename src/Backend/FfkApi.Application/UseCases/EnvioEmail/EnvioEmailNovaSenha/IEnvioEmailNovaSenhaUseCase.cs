using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.EnvioEmail.EnvioEmailNovaSenha;

public interface IEnvioEmailNovaSenhaUseCase
{
    Task<IList<ResponseDadosUsuario>?> Execute(CancellationToken cancellationToken);
}
