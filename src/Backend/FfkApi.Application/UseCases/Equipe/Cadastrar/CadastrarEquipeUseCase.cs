using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Entities;
using FfkApi.Domain.Extension;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Equipe.Cadastrar;

public class CadastrarEquipeUseCase : ICadastrarEquipeUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IOrganizacaoRepository _organizacaoRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IUsuarioRepository _usuarioRepository;
    private Domain.Entities.Usuario? _usuarioLogado = null;
    private IList<Domain.Entities.Usuario>? _usuarios = null;
    private bool jaProcurouUsuarios = false;

    public CadastrarEquipeUseCase(
        IEquipeRepository equipeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOrganizacaoRepository organizacaoRepository,
        IUsuarioLogadoService usuarioLogadoService,
        IUsuarioRepository usuarioRepository)
    {
        _equipeRepository = equipeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _organizacaoRepository = organizacaoRepository;
        _usuarioLogadoService = usuarioLogadoService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ResponseDadosEquipe> Execute(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        await Preparar(request, cancellationToken);
        await Validar(request, cancellationToken);

        var equipe = _mapper.Map<Domain.Entities.Equipe>(request);

        equipe.Membros = await CriaMembrosEquipe(request, cancellationToken);

        equipe.Organizacao = (await PegarOrganizacao(request, cancellationToken))!;

        await _equipeRepository.Adicionar(equipe, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<ResponseDadosEquipe>(equipe);
    }

    private async Task Preparar(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao))
            request.Organizacao = (await PegarUsuarioLogado(cancellationToken)).Organizacao.Nome;
    }

    private async Task Validar(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        if (await OrganizacaoEncontrada(request, cancellationToken))
        {
            mensagensDeErro.AddRange(await ValidarMembros(request, cancellationToken));
            mensagensDeErro.AddRange(await ValidarEquipe(request, cancellationToken));
        }
        else
            mensagensDeErro.Add(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA);

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private static async Task<List<string>> ValidarRequisicao(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        var validator = new CadastrarEquipeValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<bool> OrganizacaoEncontrada(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        var usuarioLogado = await PegarUsuarioLogado(cancellationToken);

        if (request.Organizacao == usuarioLogado.Organizacao.Nome)
            return true;

        if (!usuarioLogado.TemPerfilAdministrador() || !await _organizacaoRepository.ExisteOrganizacaoComNome(request.Organizacao!, cancellationToken))
            return false;

        return true;
    }

    private async Task<List<string>> ValidarMembros(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        if (request.Membros == null || request.Membros.Count == 0)
            return [];

        var emails = request.Membros.Select(m => m.Email).ToList();

        var usuarios = await PegarUsuariosPorEmails(request, cancellationToken);

        if (usuarios == null || usuarios.Count == 0)
            return [ResourceMessagesException.EMAILS_DE_USUARIOS_NAO_ENCONTRADOS.Replace("{lista}", emails!.ListaSepadadaPorVirgula())];

        var emailsNaoEncontrados = emails!
            .Except(usuarios.Select(u => u.Email))
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .ToList();

        if (emailsNaoEncontrados.Count > 0)
            return [ResourceMessagesException.EMAILS_DE_USUARIOS_NAO_ENCONTRADOS.Replace("{lista}", emailsNaoEncontrados!.ListaSepadadaPorVirgula())];

        return [];
    }

    private async Task<List<string>> ValidarEquipe(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Nome) && await _equipeRepository.ExisteEquipeComNome(request.Nome, request.Organizacao!, cancellationToken))
            return [ResourceMessagesException.NOME_DE_EQUIPE_JA_EXISTE_NA_ORGANIZACAO];

        return [];
    }

    private async Task<Domain.Entities.Organizacao?> PegarOrganizacao(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao))
            return (await PegarUsuarioLogado(cancellationToken)).Organizacao;

        return await _organizacaoRepository.PegarOrganizacaoPorNome(request.Organizacao, cancellationToken)!;
    }

    private async Task<List<MembroEquipe>> CriaMembrosEquipe(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        if (request.Membros == null || request.Membros.Count == 0)
            return [];

        var novosMembros = new List<MembroEquipe>();

        var usuarios = await PegarUsuariosPorEmails(request, cancellationToken);

        var usuariosPorEmail = usuarios!.ToDictionary(u => u.Email, StringComparer.OrdinalIgnoreCase);

        foreach (var membroEquipe in request.Membros!)
        {
            if (usuariosPorEmail.TryGetValue(membroEquipe.Email!, out var usuario))
            {
                novosMembros.Add(new MembroEquipe
                {
                    Usuario = usuario,
                    Lider = membroEquipe.Lider!.Value
                });
            }
        }

        return novosMembros;
    }

    private async Task<Domain.Entities.Usuario> PegarUsuarioLogado(CancellationToken cancellationToken)
    {
        if (_usuarioLogado != null)
            return _usuarioLogado;
        _usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        return _usuarioLogado;
    }

    private async Task<IList<Domain.Entities.Usuario>?> PegarUsuariosPorEmails(RequestCadastrarEquipe request, CancellationToken cancellationToken)
    {
        if (jaProcurouUsuarios)
            return _usuarios;

        var emails = request.Membros!.Select(m => m.Email!).ToList();
        _usuarios = await _usuarioRepository.PegarUsuariosAptosPorEmails(
            emails,
            request.Organizacao!,
            cancellationToken);

        jaProcurouUsuarios = true;

        return _usuarios;
    }
}
