namespace FfkApi.Application.UseCases.Limpeza.LimpezaLogs;

public interface ILimpezaArquivosLogUseCase
{
    Task<List<string>> Execute(CancellationToken cancellationToken);
}
