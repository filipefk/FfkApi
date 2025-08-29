using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IPerfilAcessoRepository
{
    Task<IList<PerfilAcesso>> PegarPorNomesAsync(IList<string> nomes, CancellationToken cancellationToken);
}
