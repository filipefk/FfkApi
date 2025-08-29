namespace FfkApi.Application.IUseCases;

public interface IExcluirUseCase<in TRequest>
{
    Task Execute(TRequest request, CancellationToken cancellationToken);
}
