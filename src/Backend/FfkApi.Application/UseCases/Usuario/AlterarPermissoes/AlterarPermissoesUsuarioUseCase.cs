using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Extension;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Usuario.AlterarPermissoes;

public class AlterarPermissoesUsuarioUseCase : IAlterarPermissoesUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPerfilAcessoRepository _perfilAcessoRepository;
    private readonly IPermissaoRepository _permissaoRepository;

    public AlterarPermissoesUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IPerfilAcessoRepository perfilAcessoRepository,
        IPermissaoRepository permissaoRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _perfilAcessoRepository = perfilAcessoRepository;
        _permissaoRepository = permissaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestAlterarPermissoesUsuario request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var usuario = await _usuarioRepository.PegarUsuarioPorId(Guid.Parse(request.Id!), cancellationToken);

        var perfisAcessoIguais = request.PerfisAcesso!.EquivalenteA(usuario!.PerfisAcesso.ToListNome()!);
        var permissoesIguais = request.Permissoes!.EquivalenteA(usuario.Permissoes.ToListNome()!);

        if (!perfisAcessoIguais)
        {
            usuario.PerfisAcesso.Clear();
            if (!request.PerfisAcesso!.ListaVazia())
                usuario.PerfisAcesso = await _perfilAcessoRepository.PegarPorNomesAsync(request.PerfisAcesso!, cancellationToken);
        }

        if (!permissoesIguais)
        {
            usuario.Permissoes.Clear();
            if (!request.Permissoes!.ListaVazia())
                usuario.Permissoes = await _permissaoRepository.PegarPorNomesAsync(request.Permissoes!, cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

    }

    private async Task Validar(RequestAlterarPermissoesUsuario request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarUsuarioAlteracao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarPerfisAcesso(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarPermissoes(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestAlterarPermissoesUsuario request, CancellationToken cancellationToken)
    {
        var validator = new AlterarPermissoesUsuarioValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarUsuarioAlteracao(RequestAlterarPermissoesUsuario request, CancellationToken cancellationToken)
    {
        if (!IdValidator.IdEstaValido(request.Id))
            return [];

        var usuario = await _usuarioRepository.PegarUsuarioPorId(Guid.Parse(request.Id!), cancellationToken);
        if (usuario == null || usuario.Status == Domain.Enums.StatusUsuario.Excluido)
            return [ResourceMessagesException.USUARIO_NAO_ENCONTRADO];

        var perfisAcessoIguais = request.PerfisAcesso!.EquivalenteA(usuario!.PerfisAcesso.ToListNome()!);
        var permissoesIguais = request.Permissoes!.EquivalenteA(usuario.Permissoes.ToListNome()!);

        if (perfisAcessoIguais && permissoesIguais)
            return [ResourceMessagesException.NENHUMA_ALTERACAO];

        return [];
    }

    private async Task<List<string>> ValidarPerfisAcesso(RequestAlterarPermissoesUsuario request, CancellationToken cancellationToken)
    {
        if (request.PerfisAcesso!.ListaVazia() || request.PerfisAcesso!.All(string.IsNullOrWhiteSpace))
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

    private async Task<List<string>> ValidarPermissoes(RequestAlterarPermissoesUsuario request, CancellationToken cancellationToken)
    {
        if (request.Permissoes!.ListaVazia() || request.Permissoes!.All(string.IsNullOrWhiteSpace))
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
