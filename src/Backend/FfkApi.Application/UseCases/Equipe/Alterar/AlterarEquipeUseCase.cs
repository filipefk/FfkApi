using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using FfkApi.Domain.Extension;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Equipe.Alterar;

public class AlterarEquipeUseCase : IAlterarEquipeUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IOrganizacaoRepository _organizacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private Domain.Entities.Usuario? _usuarioLogado = null;
    private Domain.Entities.Equipe? _equipe = null;
    private bool jaProcurouEquipe = false;

    public AlterarEquipeUseCase(
        IEquipeRepository equipeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOrganizacaoRepository organizacaoRepository,
        IUsuarioRepository usuarioRepository,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _equipeRepository = equipeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _organizacaoRepository = organizacaoRepository;
        _usuarioRepository = usuarioRepository;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task Execute(RequestAlterarEquipe request, CancellationToken cancellationToken)
    {
        await Preparar(request, cancellationToken);
        await Validar(request, cancellationToken);

        var equipe = await PegarEquipe(Guid.Parse(request.Id!), cancellationToken);
        _mapper.Map(request, equipe);

        if (request.Membros == null || request.Membros.Count == 0)
            equipe!.Membros.Clear();
        else
            await SincronizarMembros(equipe!, request, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Organizacao))
            equipe!.Organizacao = (await _organizacaoRepository.PegarOrganizacaoPorNome(request.Organizacao, cancellationToken))!;

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task Preparar(RequestAlterarEquipe request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao) && IdValidator.IdEstaValido(request.Id!))
        {
            var equipe = await PegarEquipe(Guid.Parse(request.Id!), cancellationToken);
            if (equipe != null)
                request.Organizacao = equipe.Organizacao.Nome;
        }
    }

    private async Task Validar(RequestAlterarEquipe request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarEquipe(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarMembros(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestAlterarEquipe request, CancellationToken cancellationToken)
    {
        var validator = new AlterarEquipeValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarOrganizacao(RequestAlterarEquipe request, CancellationToken cancellationToken)
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

    private async Task<List<string>> ValidarEquipe(RequestAlterarEquipe request, CancellationToken cancellationToken)
    {
        if (IdValidator.IdEstaValido(request.Id))
        {
            var equipe = await PegarEquipe(Guid.Parse(request.Id!), cancellationToken);
            if (equipe == null)
                return [ResourceMessagesException.EQUIPE_NAO_ENCONTRADA];

            var trocandoOrganizacao = equipe.Organizacao.Nome != request.Organizacao;

            if (trocandoOrganizacao)
            {
                List<string> mensagensDeErro = [];

                if (!request.Membros!.ListaVazia())
                    mensagensDeErro.Add(ResourceMessagesException.IMPOSSIVEL_TROCAR_ORGANIZACAO_EQUIPE_QUANDO_TEM_MEMBROS);

                if (!string.IsNullOrWhiteSpace(request.Nome) &&
                    await _equipeRepository.ExisteEquipeComNome(request.Nome, request.Organizacao!, cancellationToken))
                    mensagensDeErro.Add(ResourceMessagesException.NOME_DE_EQUIPE_JA_EXISTE_NA_ORGANIZACAO);

                if (mensagensDeErro.Count > 0)
                {
                    return mensagensDeErro;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.Nome) &&
                    request.Nome != equipe.Nome &&
                    await _equipeRepository.ExisteEquipeComNome(request.Nome, request.Organizacao!, cancellationToken))
                    return [ResourceMessagesException.NOME_DE_EQUIPE_JA_EXISTE_NA_ORGANIZACAO];
            }
        }

        return [];
    }

    private async Task<List<string>> ValidarMembros(RequestAlterarEquipe request, CancellationToken cancellationToken)
    {
        if (request.Membros == null || request.Membros.Count == 0)
            return [];

        var emails = request.Membros.Select(m => m.Email).ToList();

        var usuarios = await _usuarioRepository.PegarUsuariosAptosPorEmails(emails!, request.Organizacao!, cancellationToken);

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

    private async Task SincronizarMembros(Domain.Entities.Equipe equipe, RequestAlterarEquipe request, CancellationToken cancellationToken)
    {
        var membrosAtuais = equipe!.Membros.ToList();
        var emailsRequisicao = request.Membros?.Select(m => m.Email).Where(e => !string.IsNullOrWhiteSpace(e)).ToHashSet() ?? new HashSet<string>()!;

        foreach (var membro in membrosAtuais)
        {
            if (!emailsRequisicao.Contains(membro.Usuario.Email))
                equipe.Membros.Remove(membro);
        }

        var membrosAtuaisPorEmail = membrosAtuais.ToDictionary(m => m.Usuario.Email, m => m);

        foreach (var requestMembroEquipe in request.Membros ?? Enumerable.Empty<RequestMembroEquipe>())
        {
            if (string.IsNullOrWhiteSpace(requestMembroEquipe.Email))
                continue;

            if (!membrosAtuaisPorEmail.TryGetValue(requestMembroEquipe.Email, out var membroExistente))
            {
                var usuario = await _usuarioRepository.PegarUsuarioAptoPorEmail(requestMembroEquipe.Email!, cancellationToken);
                var novoMembro = new MembroEquipe
                {
                    Usuario = usuario!,
                    IdUsuario = usuario!.Id,
                    Equipe = equipe,
                    IdEquipe = equipe.Id,
                    Lider = requestMembroEquipe.Lider!.Value
                };
                await _equipeRepository.AdicionarMembro(novoMembro, cancellationToken);
                equipe.Membros.Add(novoMembro);
            }
            else
            {
                if (membroExistente.Lider != requestMembroEquipe.Lider!.Value)
                    membroExistente.Lider = requestMembroEquipe.Lider!.Value;
            }
        }
    }

    private async Task<Domain.Entities.Usuario> PegarUsuarioLogado(CancellationToken cancellationToken)
    {
        if (_usuarioLogado != null)
            return _usuarioLogado;
        _usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        return _usuarioLogado;
    }

    private async Task<Domain.Entities.Equipe?> PegarEquipe(Guid idEquipe, CancellationToken cancellationToken)
    {
        if (jaProcurouEquipe)
            return _equipe;

        var usuarioLogado = await PegarUsuarioLogado(cancellationToken);
        _equipe = usuarioLogado.TemPerfilAdministrador() ?
            await _equipeRepository.PegarEquipePorId(idEquipe, cancellationToken) :
            await _equipeRepository.PegarEquipePorId(idEquipe, usuarioLogado.Organizacao.Id, cancellationToken);

        jaProcurouEquipe = true;

        return _equipe;
    }
}
