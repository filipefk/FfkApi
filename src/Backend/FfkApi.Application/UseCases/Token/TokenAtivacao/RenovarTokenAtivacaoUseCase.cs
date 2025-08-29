using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Token.TokenAtivacao;

public class RenovarTokenAtivacaoUseCase : IRenovarTokenAtivacaoUseCase
{
    private readonly ITokenAtivacaoRepository _tokenAtivacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RenovarTokenAtivacaoUseCase(
        ITokenAtivacaoRepository tokenAtivacaoRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _tokenAtivacaoRepository = tokenAtivacaoRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestRenovarTokenAtivacao request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.IdUsuario);

        var usuario = await _usuarioRepository.PegarUsuarioPorId(idValido, cancellationToken);

        if (usuario == null)
            throw new NotFoundException(ResourceMessagesException.USUARIO_NAO_ENCONTRADO);

        if (usuario.Status != Domain.Enums.StatusUsuario.Inativo)
            throw new ForbiddenException(ResourceMessagesException.EMAIL_ATIVACAO_SOMENTE_USUARIOS_INATIVOS);

        var tokenAtivacao = await _tokenAtivacaoRepository.PegarTokenAtivacaoPorUsuario(usuario.Id, cancellationToken);

        if (tokenAtivacao == null)
            throw new NotFoundException(ResourceMessagesException.TOKEN_ATIVACAO_NAO_ENCONTRADO);

        await _tokenAtivacaoRepository.RedefinirDataExpiracaoEMarcarParaEnviarNovoEmail(tokenAtivacao.Id, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
