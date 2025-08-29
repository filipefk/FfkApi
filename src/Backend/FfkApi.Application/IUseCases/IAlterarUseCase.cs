namespace FfkApi.Application.IUseCases;

public interface IAlterarUseCase<in TRequest>
{
    Task Execute(TRequest request, CancellationToken cancellationToken);
}
