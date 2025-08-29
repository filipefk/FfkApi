using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Extension;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Usuario.Alterar;

public class AlterarUsuarioUseCase : IAlterarUsuarioUseCase
{
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IOrganizacaoRepository _organizacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AlterarUsuarioUseCase(
        IUsuarioLogadoService usuarioLogadoService,
        IUsuarioRepository usuarioRepository,
        IOrganizacaoRepository organizacaoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _usuarioLogadoService = usuarioLogadoService;
        _usuarioRepository = usuarioRepository;
        _organizacaoRepository = organizacaoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Execute(RequestAlterarUsuario request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var usuario = await _usuarioRepository.PegarUsuarioPorId(Guid.Parse(request.Id!), cancellationToken);
        _mapper.Map(request, usuario);

        if (!string.IsNullOrWhiteSpace(request.Organizacao))
            usuario!.Organizacao = (await _organizacaoRepository.PegarOrganizacaoPorNome(request.Organizacao!, cancellationToken))!;

        await _unitOfWork.CommitAsync(cancellationToken);

        // TODO : Logar quem alterou o usuário
    }

    private async Task Validar(RequestAlterarUsuario request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarPermissaoEStatus(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarOrganizacao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarUsuarioAlteracao(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarPermissaoEStatus(RequestAlterarUsuario request, CancellationToken cancellationToken)
    {
        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        var alterandoSeusDados = IdValidator.IdEstaValido(request.Id!) && usuarioLogado.Id == Guid.Parse(request.Id!);
        var temPermissaoCadastroUsuarios = usuarioLogado.TemPermissao("Cadastro de Usuários");

        if (!alterandoSeusDados && !temPermissaoCadastroUsuarios)
            throw new ForbiddenException(ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", "Cadastro de Usuários"));

        List<string> mensagensDeErro = [];

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (alterandoSeusDados)
            {
                var listaValida = Domain.Entities.Usuario.StatusPermitidosAoAlterarSeuProprioStatus().ListaSepadadaPorVirgula();
                if (!listaValida.Contains(request.Status!))
                    mensagensDeErro.Add(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", listaValida));
            }
            else
            {
                var listaValida = Domain.Entities.Usuario.StatusPermitidosAoAlterarStatusDeOutroUsuario().ListaSepadadaPorVirgula();
                if (!listaValida.Contains(request.Status!))
                    mensagensDeErro.Add(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", listaValida));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Organizacao) && request.Organizacao != usuarioLogado.Organizacao.Nome && !temPermissaoCadastroUsuarios)
        {
            throw new ForbiddenException(ResourceMessagesException.SEM_PERMISSAO_ALTERAR_ORGANIZACAO);
        }

        return mensagensDeErro;
    }

    private async Task<List<string>> ValidarRequisicao(RequestAlterarUsuario request, CancellationToken cancellationToken)
    {
        var validator = new AlterarUsuarioValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarUsuarioAlteracao(RequestAlterarUsuario request, CancellationToken cancellationToken)
    {
        if (!IdValidator.IdEstaValido(request.Id))
            return [];

        var usuario = await _usuarioRepository.PegarUsuarioPorId(Guid.Parse(request.Id!), cancellationToken);
        if (usuario == null || usuario.Status == Domain.Enums.StatusUsuario.Excluido)
            return [ResourceMessagesException.USUARIO_NAO_ENCONTRADO];

        List<string> mensagensDeErro = [];

        if (request.Email != usuario.Email)
        {
            var ExisteUsuarioEmail = await _usuarioRepository.ExisteUsuarioComEmail(request.Email!, cancellationToken);

            if (ExisteUsuarioEmail)
                mensagensDeErro.Add(ResourceMessagesException.EMAIL_JA_EXISTE);
        }

        if (request.Cpf != usuario.Cpf)
        {
            var organizacao = await PegarOrganizacao(request, cancellationToken);

            if (organizacao != null && await _usuarioRepository.ExisteUsuarioComCpf(request.Cpf!, organizacao.Id, cancellationToken))
                mensagensDeErro.Add(ResourceMessagesException.CPF_JA_EXISTE);
        }

        return mensagensDeErro;
    }

    private async Task<List<string>> ValidarOrganizacao(RequestAlterarUsuario request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Organizacao) || await _organizacaoRepository.ExisteOrganizacaoComNome(request.Organizacao, cancellationToken))
            return [];

        return [ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA];
    }

    private async Task<Domain.Entities.Organizacao?> PegarOrganizacao(RequestAlterarUsuario request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Organizacao))
            return await _organizacaoRepository.PegarOrganizacaoPorNome(request.Organizacao, cancellationToken);

        return (await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken)).Organizacao;
    }
}