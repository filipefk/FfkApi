using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Usuario.Ativar;

public class AtivarUsuarioUseCase : IAtivarUsuarioUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenAtivacaoRepository _tokenAtivacaoRepository;
    private readonly IEncriptadorSenha _encriptadorSenha;
    private readonly IGeradorTokenAtivacao _geradorTokenAtivacao;

    public AtivarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        ITokenAtivacaoRepository tokenAtivacaoRepository,
        IUnitOfWork unitOfWork,
        IEncriptadorSenha encriptadorSenha,
        IGeradorTokenAtivacao geradorTokenAtivacao)
    {
        _usuarioRepository = usuarioRepository;
        _tokenAtivacaoRepository = tokenAtivacaoRepository;
        _unitOfWork = unitOfWork;
        _encriptadorSenha = encriptadorSenha;
        _geradorTokenAtivacao = geradorTokenAtivacao;
    }

    public async Task Execute(RequestAtivarUsuario request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var tokenAtivacao = await _tokenAtivacaoRepository.PegarTokenAtivacaoPorToken(request.TokenAtivacao!, cancellationToken);

        if (tokenAtivacao == null)
            throw new NotFoundException(ResourceMessagesException.TOKEN_ATIVACAO_NAO_ENCONTRADO);

        if (!_geradorTokenAtivacao.TokenValido(tokenAtivacao))
            throw new ForbiddenException(ResourceMessagesException.TOKEN_ATIVACAO_EXPIRADO);

        if (request.Nome != tokenAtivacao.Usuario.Nome || request.Email != tokenAtivacao.Usuario.Email || request.Cpf != tokenAtivacao.Usuario.Cpf)
            throw new ErrorOnValidationException([ResourceMessagesException.DADOS_INVALIDOS]);

        await _usuarioRepository.AlterarSenhaEAtivar(tokenAtivacao.Usuario.Id, _encriptadorSenha.Encriptar(request.Senha!), cancellationToken);

        _tokenAtivacaoRepository.ApagarTokensDoUsuario(tokenAtivacao.Usuario.Id);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private static async Task Validar(RequestAtivarUsuario request, CancellationToken cancellationToken)
    {
        var validator = new AtivarUsuarioValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var mensagensDeErro = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }
}
