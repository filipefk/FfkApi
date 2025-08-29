using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Anexo.Alterar;

public class AlterarAnexoUseCase : IAlterarAnexoUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAnexoRepository _anexoRepository;

    public AlterarAnexoUseCase(
        IAnexoRepository anexoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _anexoRepository = anexoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Execute(RequestAlterarAnexo request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var anexo = await _anexoRepository.PegarAnexoPorId(Guid.Parse(request.Id!), cancellationToken);
        _mapper.Map(request, anexo);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task Validar(RequestAlterarAnexo request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarAnexo(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestAlterarAnexo request, CancellationToken cancellationToken)
    {
        var validator = new AlterarAnexoValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarAnexo(RequestAlterarAnexo request, CancellationToken cancellationToken)
    {
        if (IdValidator.IdEstaValido(request.Id!))
        {
            var anexo = await _anexoRepository.PegarAnexoPorId(Guid.Parse(request.Id!), cancellationToken);
            if (anexo == null)
                return [ResourceMessagesException.ANEXO_NAO_ENCONTRADO];

            if (!string.IsNullOrWhiteSpace(request.Nome) && !string.IsNullOrWhiteSpace(request.Descricao) && request.Nome == anexo.Nome && request.Descricao == anexo.Descricao)
                return [ResourceMessagesException.NENHUMA_ALTERACAO];
        }

        return [];
    }
}
