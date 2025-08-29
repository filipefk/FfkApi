using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Services.Anexo;
using FfkApi.Application.UseCases.Anexo.Cadastrar;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Feed.AdicionarAnexo;

public class AdicionarAnexoFeedUseCase : IAdicionarAnexoFeedUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IArmazenadorDeAnexoService _armazenadorDeAnexoService;
    private readonly IFeedRepository _feedRepository;

    public AdicionarAnexoFeedUseCase(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IArmazenadorDeAnexoService armazenadorDeAnexoService,
        IFeedRepository feedRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _armazenadorDeAnexoService = armazenadorDeAnexoService;
        _feedRepository = feedRepository;
    }

    public async Task<ResponseDadosAnexo> Execute(RequestAdicionarAnexoFeed request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var feed = await _feedRepository.PegarFeedPorId(Guid.Parse(request.Id!), cancellationToken);
        var anexo = await _armazenadorDeAnexoService.SalvarAsync(request, cancellationToken);
        feed!.Anexos.Add(anexo);
        await _unitOfWork.CommitAsync(cancellationToken);
        return _mapper.Map<ResponseDadosAnexo>(anexo);
    }

    private async Task Validar(RequestAdicionarAnexoFeed request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarFeed(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestAdicionarAnexoFeed request, CancellationToken cancellationToken)
    {
        var validator = new CadastrarAnexoValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarFeed(RequestAdicionarAnexoFeed request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            return [ResourceMessagesException.ID_VAZIO];
        }
        else if (!IdValidator.IdEstaValido(request.Id))
        {
            return [ResourceMessagesException.ID_INVALIDO];
        }
        else if (!await _feedRepository.ExisteFeedComId(Guid.Parse(request.Id!), cancellationToken))
        {
            return [ResourceMessagesException.FEED_NAO_ENCONTRADO];
        }

        return [];
    }
}
