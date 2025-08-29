using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.Auditoria;

namespace FfkApi.Infrastructure.Services.Auditoria;

public class AuditoriaSegurancaService : IAuditoriaSegurancaService
{
    private readonly IAuditoriaSegurancaRepository _auditoriaSegurancaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuditoriaSegurancaService(
        IAuditoriaSegurancaRepository auditoriaSegurancaRepository,
        IUnitOfWork unitOfWork)
    {
        _auditoriaSegurancaRepository = auditoriaSegurancaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(string evento, string? usuario, string? caminho, string? metodo, string? ip, string? detalhes = null)
    {
        var log = new AuditoriaSeguranca
        {
            Evento = evento,
            Usuario = usuario,
            Caminho = caminho,
            Metodo = metodo,
            EnderecoIp = ip,
            Detalhes = detalhes
        };

        await _auditoriaSegurancaRepository.Adicionar(log, CancellationToken.None);
        await _unitOfWork.CommitAsync(CancellationToken.None);
    }
}
