using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Services.Anexo;
using FfkApi.Application.UseCases.Anexo.Cadastrar;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Feed.AdicionarAnexo;

public class AdicionarAnexoFeedUseCase : IAdicionarAnexoFeedUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IArmazenadorDeAnexoService _armazenadorDeAnexoService;
    private readonly IFeedRepository _feedRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private Domain.Entities.Usuario? _usuarioLogado = null;
    private Domain.Entities.Feed? _feed = null;
    private bool _jaProcurouFeed = false;

    public AdicionarAnexoFeedUseCase(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IArmazenadorDeAnexoService armazenadorDeAnexoService,
        IFeedRepository feedRepository,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _armazenadorDeAnexoService = armazenadorDeAnexoService;
        _feedRepository = feedRepository;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<ResponseDadosAnexo> Execute(RequestAdicionarAnexoFeed request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var feed = await PegarFeed(Guid.Parse(request.Id!), cancellationToken);
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
            return [ResourceMessagesException.ID_VAZIO];

        if (!IdValidator.IdEstaValido(request.Id))
            return [ResourceMessagesException.ID_INVALIDO];

        if ((await PegarFeed(Guid.Parse(request.Id!), cancellationToken)) == null)
            return [ResourceMessagesException.FEED_NAO_ENCONTRADO];

        return [];
    }

    private async Task<Domain.Entities.Usuario> PegarUsuarioLogado(CancellationToken cancellationToken)
    {
        if (_usuarioLogado != null)
            return _usuarioLogado;
        _usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        return _usuarioLogado;
    }

    private async Task<Domain.Entities.Feed?> PegarFeed(Guid idFeed, CancellationToken cancellationToken)
    {
        if (_jaProcurouFeed)
            return _feed;

        var usuarioLogado = await PegarUsuarioLogado(cancellationToken);
        _feed = usuarioLogado.TemPerfilAdministrador() ?
            await _feedRepository.PegarFeedPorId(idFeed, cancellationToken) :
            await _feedRepository.PegarFeedPorId(idFeed, usuarioLogado.Organizacao.Id, cancellationToken);

        _jaProcurouFeed = true;

        return _feed;
    }
}
