using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Usuario.Excluir;

public class ExcluirUsuarioUseCase : IExcluirUsuarioUseCase
{
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirUsuarioUseCase(
        IUsuarioLogadoService usuarioLogadoService,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioLogadoService = usuarioLogadoService;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestExcluirUsuario request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        if (usuarioLogado.Id == idValido)
            throw new ForbiddenException(ResourceMessagesException.AUTO_EXCLUSAO);

        var usuario = await _usuarioRepository.PegarUsuarioPorId(idValido, cancellationToken);
        if (usuario == null)
            throw new NotFoundException(ResourceMessagesException.USUARIO_NAO_ENCONTRADO);

        if (usuario.Status == Domain.Enums.StatusUsuario.Excluido)
            throw new ForbiddenException(ResourceMessagesException.USUARIO_JA_EXCLUIDO);

        await _usuarioRepository.MudarStatus(usuario.Id, Domain.Enums.StatusUsuario.Excluido, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}