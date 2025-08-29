using FfkApi.Application.UseCases.Token.RefreshToken;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Token;

public class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IGeradorRefreshToken _geradorRefreshToken;
    private readonly IGeradorTokenUsuario _geradorTokenUsuario;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        IGeradorRefreshToken refreshTokenGenerator,
        IGeradorTokenUsuario geradorTokenUsuario,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _geradorRefreshToken = refreshTokenGenerator;
        _geradorTokenUsuario = geradorTokenUsuario;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseTokens> Execute(RequestNovoTokenUsuario request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var refreshToken = await _refreshTokenRepository.PegarRefreshToken(request.RefreshToken!, cancellationToken);

        // TODO : Criar testes para tokens ainda válidos mas pertencentes a usuários que foram desativados

        if (refreshToken == null)
            throw new ExpiredSessionException();

        if (!_geradorRefreshToken.TokenValido(refreshToken))
            throw new ExpiredSessionException();

        var responseToken = new ResponseTokens()
        {
            AccessToken = _geradorTokenUsuario.Gerar(refreshToken.Usuario.Id),
            RefreshToken = await CreateAndSaveRefreshToken(refreshToken.Usuario, cancellationToken)
        };

        await _unitOfWork.CommitAsync(cancellationToken);

        return responseToken;
    }

    private static async Task Validar(RequestNovoTokenUsuario request, CancellationToken cancellationToken)
    {
        var validator = new RefreshTokenValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var mensagensDeErro = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(mensagensDeErro);
        }
    }

    private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.Usuario usuario, CancellationToken cancellationToken)
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