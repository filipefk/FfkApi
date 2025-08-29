using FfkApi.Application.UseCases.Token.TokenNovaSenha;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.Token.TokenNovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioPorTokenNovaSenhaUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenNovaSenha = TokenNovaSenhaBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenNovaSenha: tokenNovaSenha, true);

        var request = new RequestPegarUsuarioPorTokenNovaSenha
        {
            TokenNovaSenha = tokenNovaSenha.Valor
        };

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(usuario.Nome));
    }

    [Test]
    public async Task Erro_Token_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenNovaSenha = TokenNovaSenhaBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var request = new RequestPegarUsuarioPorTokenNovaSenha
        {
            TokenNovaSenha = tokenNovaSenha.Valor
        };

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_NOVA_SENHA_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Token_Expirado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenNovaSenha = TokenNovaSenhaBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenNovaSenha: tokenNovaSenha, false);

        var request = new RequestPegarUsuarioPorTokenNovaSenha
        {
            TokenNovaSenha = tokenNovaSenha.Valor
        };

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_NOVA_SENHA_EXPIRADO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Token_Vazio(string? token)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenNovaSenha = TokenNovaSenhaBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenNovaSenha: tokenNovaSenha, false);

        var request = new RequestPegarUsuarioPorTokenNovaSenha
        {
            TokenNovaSenha = token
        };

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_NOVA_SENHA_VAZIO));
    }

    private static PegarUsuarioPorTokenNovaSenhaUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.TokenNovaSenha? tokenNovaSenha = null,
        bool tokenValido = false)
    {
        var tokenNovaSenhaRepository = new TokenNovaSenhaRepositoryBuilder();
        var geradorTokenNovaSenha = new GeradorTokenNovaSenhaBuilder();
        if (tokenNovaSenha != null)
        {
            tokenNovaSenhaRepository.SetupPegarTokenNovaSenhaPorTokenReturnsTokenNovaSenha(tokenNovaSenha.Valor, tokenNovaSenha, cancellationToken);
            if (tokenValido)
                geradorTokenNovaSenha.SetupTokenValidoReturnsTrue(tokenNovaSenha);
        }

        return new PegarUsuarioPorTokenNovaSenhaUseCase(
            tokenNovaSenhaRepository.Build(),
            geradorTokenNovaSenha.Build());

    }
}
