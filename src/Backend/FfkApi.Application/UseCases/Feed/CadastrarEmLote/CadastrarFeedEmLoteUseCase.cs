using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.UseCases.Feed.Cadastrar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Extension;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;

namespace FfkApi.Application.UseCases.Feed.CadastrarEmLote;

public class CadastrarFeedEmLoteUseCase : ICadastrarFeedEmLoteUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeedRepository _feedRepository;
    private readonly IOrganizacaoRepository _organizacaoRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public CadastrarFeedEmLoteUseCase(
        IFeedRepository feedRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOrganizacaoRepository organizacaoRepository,
        IEquipeRepository equipeRepository,
        IUsuarioRepository usuarioRepository)
    {
        _feedRepository = feedRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _organizacaoRepository = organizacaoRepository;
        _equipeRepository = equipeRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ResponseCadastrarEmLote<RequestCadastrarFeed, ResponseDadosFeed>> Execute(RequestCadastrarFeedEmLote requests, CancellationToken cancellationToken)
    {
        var response = new ResponseCadastrarEmLote<RequestCadastrarFeed, ResponseDadosFeed>();

        if (requests == null || requests.Feeds == null || requests.Feeds.Count == 0)
        {
            var erro = new ResponseErroCadastroLote<RequestCadastrarFeed>
            {
                Request = null!,
                MensagensDeErro = [ResourceMessagesException.LISTA_DE_FEED_VAZIA]
            };
            response.Erros.Add(erro);
            return response;
        }

        foreach (var request in requests.Feeds)
        {
            var mensagensDeErro = await Validar(request, cancellationToken);

            if (mensagensDeErro.Count > 0)
            {
                var erro = new ResponseErroCadastroLote<RequestCadastrarFeed>
                {
                    Request = request,
                    MensagensDeErro = mensagensDeErro
                };
                response.Erros.Add(erro);
                continue;
            }

            var feed = _mapper.Map<Domain.Entities.Feed>(request);

            if (!string.IsNullOrWhiteSpace(request.ExpiraEm))
                feed.ExpiraEm = DateOnly.ParseExact(request.ExpiraEm, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            if (request.VisibilidadeUsuarios != null)
            {
                foreach (var emaiUsuario in request.VisibilidadeUsuarios)
                {
                    var usuario = await _usuarioRepository.PegarUsuarioAptoPorEmail(emaiUsuario, cancellationToken);
                    feed.VisibilidadeUsuarios.Add(usuario!);
                }
            }

            if (request.VisibilidadeEquipes != null)
            {
                foreach (var nomeEquipe in request.VisibilidadeEquipes)
                {
                    var equipe = await _equipeRepository.PegarEquipePorNome(nomeEquipe, cancellationToken);
                    feed.VisibilidadeEquipes.Add(equipe!);
                }
            }

            feed.Organizacao = (await _organizacaoRepository.PegarOrganizacaoPorNome(request.Organizacao!, cancellationToken))!;

            await _feedRepository.Adicionar(feed, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            response.Cadastrados.Add(_mapper.Map<ResponseDadosFeed>(feed));
        }
        return response;
    }

    private async Task<List<string>> Validar(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarEquipes(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarUsuarios(request, cancellationToken));

        return mensagensDeErro;
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
            return [ResourceMessagesException.ORGANIZACAO_VAZIA];

        if (!await _organizacaoRepository.ExisteOrganizacaoComNome(request.Organizacao, cancellationToken))
            return [ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA];

        return [];
    }

    private async Task<List<string>> ValidarEquipes(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        if (request.VisibilidadeEquipes!.ListaVazia() || string.IsNullOrWhiteSpace(request.Organizacao))
            return [];

        List<string> mensagensDeErro = [];

        foreach (var nomeEquipe in request.VisibilidadeEquipes!)
        {
            if (!string.IsNullOrWhiteSpace(nomeEquipe) && !await _equipeRepository.ExisteEquipeComNome(nomeEquipe, request.Organizacao, cancellationToken))
                mensagensDeErro.Add(ResourceMessagesException.NOME_EQUIPE_NAO_ENCONTRADO.Replace("{nome-equipe}", nomeEquipe));
        }

        return mensagensDeErro;
    }

    private async Task<List<string>> ValidarUsuarios(RequestCadastrarFeed request, CancellationToken cancellationToken)
    {
        if (request.VisibilidadeUsuarios!.ListaVazia() || string.IsNullOrWhiteSpace(request.Organizacao))
            return [];

        List<string> mensagensDeErro = [];

        foreach (var emailUsuario in request.VisibilidadeUsuarios!)
        {
            if (!string.IsNullOrWhiteSpace(emailUsuario) && !await _usuarioRepository.ExisteUsuarioAptoComEmailNaOrganizacao(emailUsuario, request.Organizacao!, cancellationToken))
                mensagensDeErro.Add(ResourceMessagesException.EMAIL_USUARIO_NAO_ENCONTRADO.Replace("{email}", emailUsuario));
        }

        return mensagensDeErro;
    }
}
