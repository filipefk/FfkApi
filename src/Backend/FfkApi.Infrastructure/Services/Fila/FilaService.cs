using FfkApi.Domain.Entities;
using FfkApi.Domain.Services.Fila;
using FfkApi.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FfkApi.Infrastructure.Services.Fila;

public class FilaService : IFilaService
{
    private readonly FfkApiDbContext _dbContext;

    public FilaService(FfkApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FilaItem?> PegarProximoItemDaFilaAsync(Guid idFila, CancellationToken cancellationToken)
    {
        return await _dbContext.FilaItens
            .Where(fi => fi.IdFila == idFila)
            .OrderBy(fi => fi.Posicao)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task CriarFilaParaEquipesSemFilaAsync(CancellationToken cancellationToken)
    {
        var equipesSemFila = await _dbContext.Equipes
            .Where(e => e.Fila == null)
            .ToListAsync(cancellationToken);

        foreach (var equipe in equipesSemFila)
        {
            var novaFila = new Domain.Entities.Fila
            {
                Equipe = equipe,
            };
            equipe.Fila = novaFila;

            await _dbContext.Filas.AddAsync(novaFila, cancellationToken);
        }
    }
}
