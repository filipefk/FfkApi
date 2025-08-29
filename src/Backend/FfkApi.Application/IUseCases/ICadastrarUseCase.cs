namespace FfkApi.Application.IUseCases;

public interface ICadastrarUseCase<in TRequest, TResponse>
{
    Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken);
}
