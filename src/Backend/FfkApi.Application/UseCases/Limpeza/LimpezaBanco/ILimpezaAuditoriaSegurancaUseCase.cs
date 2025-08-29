namespace FfkApi.Application.UseCases.Limpeza.LimpezaBanco;

public interface ILimpezaAuditoriaSegurancaUseCase
{
    Task<int> Execute(CancellationToken cancellationToken);
}
