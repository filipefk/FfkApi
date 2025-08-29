namespace FfkApi.Application.IUseCases;

public interface IPegarUseCase<in TRequest, TResponse>
{
    Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken);
}
