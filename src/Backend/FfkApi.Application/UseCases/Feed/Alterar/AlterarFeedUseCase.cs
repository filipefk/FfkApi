using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Extension;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Feed.Alterar;

public class AlterarFeedUseCase : IAlterarFeedUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeedRepository _feedRepository;
    private readonly IOrganizacaoRepository _organizacaoRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private Domain.Entities.Usuario? _usuarioLogado = null;
    private Domain.Entities.Feed? _feed = null;
    private bool jaProcurouFeed = false;
    private IList<Domain.Entities.Usuario>? _usuarios = null;
    private bool jaProcurouUsuarios = false;
    private IList<Domain.Entities.Equipe>? _equipes = null;
    private bool jaProcurouEquipes = false;

    public AlterarFeedUseCase(
        IFeedRepository feedRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOrganizacaoRepository organizacaoRepository,
        IEquipeRepository equipeRepository,
        IUsuarioRepository usuarioRepository,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _feedRepository = feedRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _organizacaoRepository = organizacaoRepository;
        _equipeRepository = equipeRepository;
        _usuarioRepository = usuarioRepository;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task Execute(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        await Preparar(request, cancellationToken);
        await Validar(request, cancellationToken);

        var feed = await PegarFeed(Guid.Parse(request.Id!), cancellationToken);
        _mapper.Map(request, feed);

        if (!string.IsNullOrWhiteSpace(request.ExpiraEm))
            feed!.ExpiraEm = DateOnly.ParseExact(request.ExpiraEm, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        feed!.VisibilidadeUsuarios.Clear();
        if (!request.VisibilidadeUsuarios!.ListaNullOrWhiteSpace())
            feed.VisibilidadeUsuarios = (await PegarUsuariosPorEmails(request, cancellationToken))!;

        feed.VisibilidadeEquipes.Clear();
        if (!request.VisibilidadeEquipes!.ListaNullOrWhiteSpace())
            feed.VisibilidadeEquipes = (await PegarEquipesPorNome(request, cancellationToken))!;

        if (!string.IsNullOrWhiteSpace(request.Organizacao))
            feed!.Organizacao = (await _organizacaoRepository.PegarOrganizacaoPorNome(request.Organizacao, cancellationToken))!;

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task Preparar(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao) && IdValidator.IdEstaValido(request.Id!))
        {
            var feed = await PegarFeed(Guid.Parse(request.Id!), cancellationToken);
            if (feed != null)
                request.Organizacao = feed.Organizacao.Nome;
        }
    }

    private async Task Validar(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarFeed(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarEquipes(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarUsuarios(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        var validator = new AlterarFeedValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarFeed(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        if (IdValidator.IdEstaValido(request.Id))
        {
            var feed = await PegarFeed(Guid.Parse(request.Id!), cancellationToken);
            if (feed == null)
                return [ResourceMessagesException.FEED_NAO_ENCONTRADO];

            var trocandoOrganizacao = feed.Organizacao.Nome != request.Organizacao;

            if (trocandoOrganizacao)
            {
                List<string> mensagensDeErro = [];

                if (!request.VisibilidadeEquipes!.ListaVazia())
                    mensagensDeErro.Add(ResourceMessagesException.IMPOSSIVEL_TROCAR_ORGANIZACAO_FEED_QUANDO_TEM_VISIBILIDADE_EQUIPES);

                if (!request.VisibilidadeUsuarios!.ListaVazia())
                    mensagensDeErro.Add(ResourceMessagesException.IMPOSSIVEL_TROCAR_ORGANIZACAO_FEED_QUANDO_TEM_VISIBILIDADE_USUARIOS);

                if (mensagensDeErro.Count > 0)
                {
                    return mensagensDeErro;
                }
            }

        }
        return [];
    }

    private async Task<List<string>> ValidarOrganizacao(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao) || !IdValidator.IdEstaValido(request.Id!))
            return [];

        var usuarioLogado = await PegarUsuarioLogado(cancellationToken);

        if (request.Organizacao == usuarioLogado.Organizacao.Nome)
            return [];

        if (!usuarioLogado.TemPerfilAdministrador() || !await _organizacaoRepository.ExisteOrganizacaoComNome(request.Organizacao!, cancellationToken))
            return [ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA];

        return [];
    }

    private async Task<List<string>> ValidarEquipes(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        if (request.VisibilidadeEquipes!.ListaNullOrWhiteSpace())
            return [];

        var nomesEquipes = request.VisibilidadeEquipes!.Where(nome => !string.IsNullOrWhiteSpace(nome)).ToList();

        var equipes = await PegarEquipesPorNome(request, cancellationToken);

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

    private async Task<List<string>> ValidarUsuarios(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        if (request.VisibilidadeUsuarios!.ListaNullOrWhiteSpace())
            return [];

        var emailsUsuarios = request.VisibilidadeUsuarios!.Where(email => !string.IsNullOrWhiteSpace(email)).ToList();

        var usuarios = await PegarUsuariosPorEmails(request, cancellationToken);

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

    private async Task<Domain.Entities.Usuario> PegarUsuarioLogado(CancellationToken cancellationToken)
    {
        if (_usuarioLogado != null)
            return _usuarioLogado;
        _usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        return _usuarioLogado;
    }

    private async Task<Domain.Entities.Feed?> PegarFeed(Guid idFeed, CancellationToken cancellationToken)
    {
        if (jaProcurouFeed)
            return _feed;

        var usuarioLogado = await PegarUsuarioLogado(cancellationToken);
        _feed = usuarioLogado.TemPerfilAdministrador() ?
            await _feedRepository.PegarFeedPorId(idFeed, cancellationToken) :
            await _feedRepository.PegarFeedPorId(idFeed, usuarioLogado.Organizacao.Id, cancellationToken);

        jaProcurouFeed = true;

        return _feed;
    }

    private async Task<IList<Domain.Entities.Usuario>?> PegarUsuariosPorEmails(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        if (jaProcurouUsuarios)
            return _usuarios;

        var emailsUsuarios = request.VisibilidadeUsuarios!.Where(email => !string.IsNullOrWhiteSpace(email)).ToList();

        _usuarios = await _usuarioRepository.PegarUsuariosAptosPorEmails(
            emailsUsuarios,
            request.Organizacao!,
            cancellationToken);

        jaProcurouUsuarios = true;

        return _usuarios;
    }

    private async Task<IList<Domain.Entities.Equipe>?> PegarEquipesPorNome(RequestAlterarFeed request, CancellationToken cancellationToken)
    {
        if (jaProcurouEquipes)
            return _equipes;

        var nomesEquipes = request.VisibilidadeEquipes!.Where(nome => !string.IsNullOrWhiteSpace(nome)).ToList();

        _equipes = await _equipeRepository.PegarPorNomesNaOrganizacao(
            nomesEquipes,
            request.Organizacao!,
            cancellationToken);

        jaProcurouEquipes = true;

        return _equipes;
    }
}
