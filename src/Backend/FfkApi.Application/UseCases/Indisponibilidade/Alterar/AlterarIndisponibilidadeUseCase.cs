using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Indisponibilidade.Alterar;

public class AlterarIndisponibilidadeUseCase : IAlterarIndisponibilidadeUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIndisponibilidadeRepository _indisponibilidadeRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IUsuarioRepository _usuarioRepository;
    private Domain.Entities.Usuario? _usuarioLogado = null;
    private Domain.Entities.Usuario? _usuarioIndisponibilidade = null;
    private bool jaProcurouUsuarioIndisponibilidade = false;
    private Domain.Entities.Indisponibilidade? _indisponibilidade = null;
    private bool jaProcurouIndisponibilidade = false;

    public AlterarIndisponibilidadeUseCase(
        IIndisponibilidadeRepository indisponibilidadeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService,
        IUsuarioRepository usuarioRepository)
    {
        _indisponibilidadeRepository = indisponibilidadeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task Execute(RequestAlterarIndisponibilidade request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var indisponibilidade = await PegarIndisponibilidade(Guid.Parse(request.Id!), cancellationToken);
        var usuarioIndisponibilidade = await PegarUsuarioIndisponibilidade(request.Usuario!, cancellationToken);
        _mapper.Map(request, indisponibilidade);
        indisponibilidade!.DataInicial = StringParaDateOnly(request.DataInicial!);
        indisponibilidade.DataFinal = StringParaDateOnly(request.DataFinal!);
        indisponibilidade.IdUsuario = usuarioIndisponibilidade!.Id;
        indisponibilidade.Usuario = usuarioIndisponibilidade;

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task Validar(
        RequestAlterarIndisponibilidade request,
        CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarUsuarioIndisponibilidadeEPermissao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarIndisponibilidade(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarUsuarioIndisponibilidadeEPermissao(
        RequestAlterarIndisponibilidade request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return [];

        var usuarioIndisponibilidade = await PegarUsuarioIndisponibilidade(request.Usuario, cancellationToken);

        if (usuarioIndisponibilidade == null)
            return [ResourceMessagesException.USUARIO_NAO_ENCONTRADO];

        var usuarioLogado = await PegarUsuarioLogado(cancellationToken);

        var alterandoSeusDados = usuarioLogado.Id == usuarioIndisponibilidade.Id;
        var temPermissaoCadastroIndisponibilidade = usuarioLogado.TemPermissao("Cadastro de Indisponibilidades");

        if (!alterandoSeusDados && !temPermissaoCadastroIndisponibilidade)
            throw new ForbiddenException(ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", "Cadastro de Indisponibilidades"));

        return [];
    }

    private async Task<List<string>> ValidarRequisicao(RequestAlterarIndisponibilidade request, CancellationToken cancellationToken)
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarIndisponibilidade(
        RequestAlterarIndisponibilidade request,
        CancellationToken cancellationToken)
    {
        if (IdValidator.IdEstaValido(request.Id!) && (await PegarIndisponibilidade(Guid.Parse(request.Id!), cancellationToken)) == null)
            return [ResourceMessagesException.INDISPONIBILIDADE_NAO_ENCONTRADA];

        var usuarioIndisponibilidade = await PegarUsuarioIndisponibilidade(request.Usuario!, cancellationToken);

        if (usuarioIndisponibilidade == null)
            return [];

        if (IdValidator.IdEstaValido(request.Id) && DatasValidas(request))
        {
            if (await _indisponibilidadeRepository.ExisteIndisponibilidadeParaUsuarioNoPeriodo(
                usuarioIndisponibilidade.Id,
                StringParaDateOnly(request.DataInicial!),
                StringParaDateOnly(request.DataFinal!),
                Guid.Parse(request.Id!),
                cancellationToken))
            {
                return [ResourceMessagesException.JA_EXISTE_INDISPONIBILIDADE_NO_PERIODO];
            }
        }

        return [];
    }

    private static bool DatasValidas(RequestAlterarIndisponibilidade request)
    {
        return DataValidator.DataValida(request.DataInicial) &&
            DataValidator.DataValida(request.DataFinal) &&
            DataValidator.DataFinalMaiorOuIgualDataInicial(request.DataInicial!, request.DataFinal!);
    }

    private static DateOnly StringParaDateOnly(string data)
    {
        return DateOnly.ParseExact(data, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }

    private async Task<Domain.Entities.Usuario> PegarUsuarioLogado(CancellationToken cancellationToken)
    {
        if (_usuarioLogado != null)
            return _usuarioLogado;
        _usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        return _usuarioLogado;
    }

    private async Task<Domain.Entities.Usuario?> PegarUsuarioIndisponibilidade(string emailUsuario, CancellationToken cancellationToken)
    {
        if (jaProcurouUsuarioIndisponibilidade)
            return _usuarioIndisponibilidade;

        if (string.IsNullOrWhiteSpace(emailUsuario))
            return null;

        var usuarioLogado = await PegarUsuarioLogado(cancellationToken);

        _usuarioIndisponibilidade = usuarioLogado.TemPerfilAdministrador() ?
            await _usuarioRepository.PegarUsuarioAptoPorEmail(emailUsuario!, cancellationToken) :
            await _usuarioRepository.PegarUsuarioAptoPorEmail(emailUsuario!, usuarioLogado.Organizacao.Id, cancellationToken);

        jaProcurouUsuarioIndisponibilidade = true;

        return _usuarioIndisponibilidade;
    }

    private async Task<Domain.Entities.Indisponibilidade?> PegarIndisponibilidade(Guid idIndisponibilidade, CancellationToken cancellationToken)
    {
        if (jaProcurouIndisponibilidade)
            return _indisponibilidade;

        var usuarioLogado = await PegarUsuarioLogado(cancellationToken);
        _indisponibilidade = usuarioLogado.TemPerfilAdministrador() ?
            await _indisponibilidadeRepository.PegarIndisponibilidadePorId(idIndisponibilidade, cancellationToken) :
            await _indisponibilidadeRepository.PegarIndisponibilidadePorId(idIndisponibilidade, usuarioLogado.Organizacao.Id, cancellationToken);

        jaProcurouIndisponibilidade = true;

        return _indisponibilidade;
    }
}
