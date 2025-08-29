using FfkApi.Application.UseCases.Token;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.Token.RefreshToken;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class RefreshTokenUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var refreshToken = RefreshTokenBuilder.Build(usuario);

        var request = RequestNovoTokenUsuarioBuilder.Build(refreshToken);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, refreshToken: refreshToken, true);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.AccessToken));
        Assert.That(!string.IsNullOrWhiteSpace(response.RefreshToken));
    }

    [Test]
    public async Task Erro_Token_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var refreshToken = RefreshTokenBuilder.Build(usuario);

        var request = RequestNovoTokenUsuarioBuilder.Build(refreshToken);

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ExpiredSessionException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SESSAO_EXPIRADA));
    }

    [Test]
    public async Task Erro_Token_Expirado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var refreshToken = RefreshTokenBuilder.Build(usuario);

        var request = RequestNovoTokenUsuarioBuilder.Build(refreshToken);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, refreshToken: refreshToken, false);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ExpiredSessionException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SESSAO_EXPIRADA));
    }

    private static RefreshTokenUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.RefreshToken? refreshToken = null,
        bool tokenValido = false)
    {
        var refreshTokenRepository = new RefreshTokenRepositoryBuilder();
        var geradorRefreshToken = new GeradorRefreshTokenBuilder();

        if (refreshToken != null)
        {
            refreshTokenRepository.SetupPegarRefreshTokenReturnsRefreshToken(refreshToken.Valor, refreshToken, cancellationToken);
            if (tokenValido)
                geradorRefreshToken.SetupTokenValidoReturnsTrue(refreshToken);
        }

        return new RefreshTokenUseCase(
            refreshTokenRepository.Build(),
            geradorRefreshToken.Build(),
            GeradorTokenUsuarioBuilder.Build(),
            UnitOfWorkBuilder.Build());
    }
}
