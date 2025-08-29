using FfkApi.Communication.Requests;

namespace FfkApi.Application.UseCases.Usuario.TrocarSenha;

public interface ITrocarSenhaUseCase
{
    Task Execute(RequestTrocarSenha request, CancellationToken cancellationToken);
}