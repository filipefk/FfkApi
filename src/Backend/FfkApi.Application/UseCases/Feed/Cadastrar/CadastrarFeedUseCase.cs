using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Extension;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Feed.Cadastrar;

public class CadastrarFeedUseCase : ICadastrarFeedUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeedRepository _feedRepository;
    private readonly IOrganizacaoRepository _organizacaoRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public CadastrarFeedUseCase(
        IFeedRepository feedRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOrganizacaoRepository organizacaoRepository,
        IUsuarioLogadoService usuarioLogadoService,
        IEquipeRepository equipeRepository,
        IUsuarioRepository usuarioRepository)
    {
        _feedRepository = feedRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _organizacaoRepository = organizacaoRepository;
        _usuarioLogadoService = usuarioLogadoService;
        _equipeRepository = equipeRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ResponseDadosFeed> Execute(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var feed = _mapper.Map<Domain.Entities.Feed>(request);

        if (!string.IsNullOrWhiteSpace(request.ExpiraEm))
            feed.ExpiraEm = DateOnly.ParseExact(request.ExpiraEm, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        if (request.VisibilidadeUsuarios != null && request.VisibilidadeUsuarios.Count > 0)
            feed.VisibilidadeUsuarios = await _usuarioRepository.PegarUsuariosAptosPorEmails(request.VisibilidadeUsuarios, request.Organizacao!, cancellationToken);

        if (request.VisibilidadeEquipes != null && request.VisibilidadeEquipes.Count > 0)
            feed.VisibilidadeEquipes = await _equipeRepository.PegarPorNomesNaOrganizacao(request.VisibilidadeEquipes, request.Organizacao!, cancellationToken);

        feed.Organizacao = (await PegarOrganizacao(request, cancellationToken))!;

        await _feedRepository.Adicionar(feed, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<ResponseDadosFeed>(feed);
    }

    private async Task Validar(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarEquipes(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarUsuarios(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        var validator = new CadastrarFeedValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarOrganizacao(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao))
        {
            request.Organizacao = (await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken)).Organizacao.Nome;
            return [];
        }

        if (await _organizacaoRepository.ExisteOrganizacaoComNome(request.Organizacao, cancellationToken))
            return [];

        return [ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA];
    }

    private async Task<List<string>> ValidarEquipes(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        if (request.VisibilidadeEquipes!.ListaNullOrWhiteSpace())
            return [];

        var nomesEquipes = request.VisibilidadeEquipes!.Where(nome => !string.IsNullOrWhiteSpace(nome)).ToList();

        var equipes = await _equipeRepository.PegarPorNomesNaOrganizacao(nomesEquipes, request.Organizacao!, cancellationToken);

        if (equipes == null || equipes.Count == 0)
            return [ResourceMessagesException.NOMES_DE_EQUIPES_NAO_ENCONTRADOS.Replace("{lista}", nomesEquipes.ListaSepadadaPorVirgula())];

        var nomesEquipesNaoEncontrados = nomesEquipes
            .Except(equipes.Select(e => e.Nome))
            .Where(nome => !string.IsNullOrWhiteSpace(nome))
            .ToList();

        if (nomesEquipesNaoEncontrados.Count > 0)
            return [ResourceMessagesException.NOMES_DE_EQUIPES_NAO_ENCONTRADOS.Replace("{lista}", nomesEquipesNaoEncontrados.ListaSepadadaPorVirgula())];

        return [];
    }

    private async Task<List<string>> ValidarUsuarios(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        if (request.VisibilidadeUsuarios!.ListaNullOrWhiteSpace())
            return [];

        var emailsUsuarios = request.VisibilidadeUsuarios!.Where(email => !string.IsNullOrWhiteSpace(email)).ToList();

        var usuarios = await _usuarioRepository.PegarUsuariosAptosPorEmails(emailsUsuarios, request.Organizacao!, cancellationToken);

        if (usuarios == null || usuarios.Count == 0)
            return [ResourceMessagesException.EMAILS_DE_USUARIOS_NAO_ENCONTRADOS.Replace("{lista}", emailsUsuarios.ListaSepadadaPorVirgula())];

        var emailsNaoEncontrados = emailsUsuarios
            .Except(usuarios.Select(u => u.Email))
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .ToList();

        if (emailsNaoEncontrados.Count > 0)
            return [ResourceMessagesException.EMAILS_DE_USUARIOS_NAO_ENCONTRADOS.Replace("{lista}", emailsNaoEncontrados.ListaSepadadaPorVirgula())];

        return [];
    }

    private async Task<Domain.Entities.Organizacao?> PegarOrganizacao(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao))
            return (await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken)).Organizacao;

        return await _organizacaoRepository.PegarOrganizacaoPorNome(request.Organizacao, cancellationToken)!;
    }

}
