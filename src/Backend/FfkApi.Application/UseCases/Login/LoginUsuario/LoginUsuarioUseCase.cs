using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Login.LoginUsuario;

public class LoginUsuarioUseCase : ILoginUsuarioUseCase
{
    private readonly IGeradorTokenUsuario _geradorTokenUsuario;
    private readonly IEncriptadorSenha _encriptadorSenha;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IGeradorRefreshToken _geradorRefreshToken;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUsuarioUseCase(IUsuarioRepository usuarioRepository,
        IEncriptadorSenha passwordEncryptor,
        IGeradorTokenUsuario geradorTokenUsuario,
        IGeradorRefreshToken refreshTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _encriptadorSenha = passwordEncryptor;
        _geradorTokenUsuario = geradorTokenUsuario;
        _geradorRefreshToken = refreshTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseLoginUsuario> Execute(RequestLoginUsuario request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var usuario = await _usuarioRepository.PegarUsuarioAptoPorEmail(request.Email!, cancellationToken);

        if (usuario is null)
            throw new InvalidLoginUsuarioException();

        var validatePassword = _encriptadorSenha.SenhaValida(request.Senha!, usuario.Senha!);

        if (!validatePassword)
            throw new InvalidLoginUsuarioException();

        var responseToken = new ResponseTokens()
        {
            AccessToken = _geradorTokenUsuario.Gerar(usuario.Id),
            RefreshToken = await CriarESalvarRefreshToken(usuario, cancellationToken)
        };

        await _unitOfWork.CommitAsync(cancellationToken);

        return new ResponseLoginUsuario
        {
            Nome = usuario.Nome,
            Tokens = responseToken
        };
    }

    private static async Task Validar(RequestLoginUsuario request, CancellationToken cancellationToken)
    {
        var validator = new LoginUsuarioValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var mensagensDeErro = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }

    private async Task<string> CriarESalvarRefreshToken(Domain.Entities.Usuario usuario, CancellationToken cancellationToken)
    {
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Valor = _geradorRefreshToken.Gerar(),
            IdUsuario = usuario.Id
        };

        await _refreshTokenRepository.SalvarNovoRefreshToken(refreshToken, cancellationToken);

        return refreshToken.Valor;
    }

}