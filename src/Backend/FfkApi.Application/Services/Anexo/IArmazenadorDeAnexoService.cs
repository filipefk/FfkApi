using FfkApi.Communication.Requests;

namespace FfkApi.Application.Services.Anexo;

public interface IArmazenadorDeAnexoService
{
    Task<Domain.Entities.Anexo> SalvarAsync(RequestCadastrarAnexo request, CancellationToken cancellationToken);

    Task ExcluirAsync(Domain.Entities.Anexo anexo, CancellationToken cancellationToken);
}
