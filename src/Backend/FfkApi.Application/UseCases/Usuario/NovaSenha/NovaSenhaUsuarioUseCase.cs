using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Usuario.NovaSenha;

public class NovaSenhaUsuarioUseCase : INovaSenhaUsuarioUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenNovaSenhaRepository _tokenNovaSenhaRepository;
    private readonly IEncriptadorSenha _encriptadorSenha;
    private readonly IGeradorTokenNovaSenha _geradorTokenNovaSenha;

    public NovaSenhaUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        ITokenNovaSenhaRepository tokenNovaSenhaRepository,
        IUnitOfWork unitOfWork,
        IEncriptadorSenha encriptadorSenha,
        IGeradorTokenNovaSenha geradorTokenNovaSenha)
    {
        _usuarioRepository = usuarioRepository;
        _tokenNovaSenhaRepository = tokenNovaSenhaRepository;
        _unitOfWork = unitOfWork;
        _encriptadorSenha = encriptadorSenha;
        _geradorTokenNovaSenha = geradorTokenNovaSenha;
    }

    public async Task Execute(RequestNovaSenhaUsuario request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var tokenNovaSenha = await _tokenNovaSenhaRepository.PegarTokenNovaSenhaPorToken(request.TokenNovaSenha!, cancellationToken);

        if (tokenNovaSenha == null)
            throw new NotFoundException(ResourceMessagesException.TOKEN_NOVA_SENHA_NAO_ENCONTRADO);

        if (!_geradorTokenNovaSenha.TokenValido(tokenNovaSenha))
            throw new ForbiddenException(ResourceMessagesException.TOKEN_NOVA_SENHA_EXPIRADO);

        if (request.Nome != tokenNovaSenha.Usuario.Nome || request.Email != tokenNovaSenha.Usuario.Email || request.Cpf != tokenNovaSenha.Usuario.Cpf)
            throw new ErrorOnValidationException([ResourceMessagesException.DADOS_INVALIDOS]);

        await _usuarioRepository.AlterarSenha(tokenNovaSenha.Usuario.Id, _encriptadorSenha.Encriptar(request.NovaSenha!), cancellationToken);

        _tokenNovaSenhaRepository.ApagarTokensDoUsuario(tokenNovaSenha.Usuario.Id);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private static async Task Validar(RequestNovaSenhaUsuario request, CancellationToken cancellationToken)
    {
        var validator = new NovaSenhaUsuarioValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var mensagensDeErro = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }
}
