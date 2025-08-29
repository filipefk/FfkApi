using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Services.Fila;

public interface IFilaService
{
    Task<FilaItem?> PegarProximoItemDaFilaAsync(Guid idFila, CancellationToken cancellationToken);
    Task CriarFilaParaEquipesSemFilaAsync(CancellationToken cancellationToken);
}
