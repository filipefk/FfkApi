using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IPermissaoRepository
{
    Task<IList<Permissao>> PegarPorNomesAsync(IList<string> nomes, CancellationToken cancellationToken);
}
