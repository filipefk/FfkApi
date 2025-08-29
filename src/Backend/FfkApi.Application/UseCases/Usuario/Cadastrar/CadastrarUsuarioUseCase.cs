using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Extension;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using FluentValidation.Results;

namespace FfkApi.Application.UseCases.Usuario.Cadastrar;

public class CadastrarUsuarioUseCase : ICadastrarUsuarioUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPerfilAcessoRepository _perfilAcessoRepository;
    private readonly IPermissaoRepository _permissaoRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IOrganizacaoRepository _organizacaoRepository;

    public CadastrarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IPerfilAcessoRepository perfilAcessoRepository,
        IPermissaoRepository permissaoRepository,
        IOrganizacaoRepository organizacaoRepository,
        IUsuarioLogadoService usuarioLogadoService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _perfilAcessoRepository = perfilAcessoRepository;
        _permissaoRepository = permissaoRepository;
        _organizacaoRepository = organizacaoRepository;
        _usuarioLogadoService = usuarioLogadoService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDadosUsuario> Execute(RequestCadastrarUsuario request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var organizacao = await PegarOrganizacao(request, cancellationToken);

        var usuario = _mapper.Map<Domain.Entities.Usuario>(request);
        usuario.Status = Domain.Enums.StatusUsuario.Inativo;
        usuario.IdOrganizacao = organizacao!.Id;
        if (request.PerfisAcesso?.Count > 0)
            usuario.PerfisAcesso = await _perfilAcessoRepository.PegarPorNomesAsync(request.PerfisAcesso, cancellationToken);

        if (request.Permissoes?.Count > 0)
            usuario.Permissoes = await _permissaoRepository.PegarPorNomesAsync(request.Permissoes, cancellationToken);

        await _usuarioRepository.Adicionar(usuario, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        usuario.Organizacao = organizacao;

        return _mapper.Map<ResponseDadosUsuario>(usuario);

        // TODO : Logar quem cadastrou o usuário
    }

    private async Task<Domain.Entities.Organizacao?> PegarOrganizacao(RequestCadastrarUsuario request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao))
            return (await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken)).Organizacao;

        return (await _organizacaoRepository.PegarOrganizacaoPorNome(request.Organizacao, cancellationToken))!;
    }

    private async Task Validar(RequestCadastrarUsuario request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarPerfisAcesso(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarPermissoes(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarOrganizacao(RequestCadastrarUsuario request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao) || await _organizacaoRepository.ExisteOrganizacaoComNome(request.Organizacao, cancellationToken))
            return [];

        return [ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA];
    }

    private async Task<List<string>> ValidarRequisicao(RequestCadastrarUsuario request, CancellationToken cancellationToken)
    {
        var validator = new CadastrarUsuarioValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Email) && await _usuarioRepository.ExisteUsuarioComEmail(request.Email, cancellationToken))
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_JA_EXISTE));

        if (!string.IsNullOrWhiteSpace(request.Cpf))
        {
            var organizacao = await PegarOrganizacao(request, cancellationToken);

            if (organizacao != null && await _usuarioRepository.ExisteUsuarioComCpf(request.Cpf!, organizacao.Id, cancellationToken))
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.CPF_JA_EXISTE));
        }

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarPerfisAcesso(RequestCadastrarUsuario request, CancellationToken cancellationToken)
    {
        if (request.PerfisAcesso!.ListaNullOrWhiteSpace())
            return [];

        var perfis = await _perfilAcessoRepository.PegarPorNomesAsync(request.PerfisAcesso!, cancellationToken);

        if (perfis == null || perfis.Count == 0)
            return [ResourceMessagesException.PERFIS_ACESSO_NAO_ENCONTRADOS.Replace("{lista}", request.PerfisAcesso!.ListaSepadadaPorVirgula())];

        var perfisNaoEncontrados = request.PerfisAcesso!
            .Except(perfis.Select(p => p.Nome))
            .Where(nome => !string.IsNullOrWhiteSpace(nome))
            .ToList();

        if (perfisNaoEncontrados.Count > 0)
            return [ResourceMessagesException.PERFIS_ACESSO_NAO_ENCONTRADOS.Replace("{lista}", perfisNaoEncontrados.ListaSepadadaPorVirgula())];

        return [];
    }

    private async Task<List<string>> ValidarPermissoes(RequestCadastrarUsuario request, CancellationToken cancellationToken)
    {
        if (request.Permissoes!.ListaNullOrWhiteSpace())
            return [];

        var permissoes = await _permissaoRepository.PegarPorNomesAsync(request.Permissoes!, cancellationToken);

        if (permissoes == null || permissoes.Count == 0)
            return [ResourceMessagesException.PERMISSOES_NAO_ENCONTRADAS.Replace("{lista}", request.Permissoes!.ListaSepadadaPorVirgula())];

        var permissoesNaoEncontradas = request.Permissoes!
            .Except(permissoes.Select(p => p.Nome))
            .Where(nome => !string.IsNullOrWhiteSpace(nome))
            .ToList();

        if (permissoesNaoEncontradas.Count > 0)
            return [ResourceMessagesException.PERMISSOES_NAO_ENCONTRADAS.Replace("{lista}", permissoesNaoEncontradas.ListaSepadadaPorVirgula())];

        return [];
    }
}