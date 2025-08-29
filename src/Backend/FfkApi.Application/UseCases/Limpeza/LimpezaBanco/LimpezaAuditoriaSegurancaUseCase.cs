using FfkApi.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace FfkApi.Application.UseCases.Limpeza.LimpezaBanco;

public class LimpezaAuditoriaSegurancaUseCase : ILimpezaAuditoriaSegurancaUseCase
{

    private readonly IAuditoriaSegurancaRepository _auditoriaSegurancaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly uint _limpezaAuditoriaSegurancaDias;

    public LimpezaAuditoriaSegurancaUseCase(
        IAuditoriaSegurancaRepository auditoriaSegurancaRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _auditoriaSegurancaRepository = auditoriaSegurancaRepository;
        _unitOfWork = unitOfWork;
        _limpezaAuditoriaSegurancaDias = configuration
            .GetSection("Configuracoes:Limpeza:LimpezaBanco:LimpezaAuditoriaSegurancaDias")
            .Get<uint>();
    }

    public async Task<int> Execute(CancellationToken cancellationToken)
    {
        var quant = await _auditoriaSegurancaRepository.Limpar(_limpezaAuditoriaSegurancaDias, cancellationToken);

        if (quant > 0)
        {
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        return quant;
    }
}
