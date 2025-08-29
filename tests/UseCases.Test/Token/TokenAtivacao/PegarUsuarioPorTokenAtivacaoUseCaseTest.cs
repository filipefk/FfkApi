using FfkApi.Application.UseCases.Token.TokenAtivacao;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.Token.TokenAtivacao;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioPorTokenAtivacaoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenAtivacao: tokenAtivacao, true);

        var request = new RequestPegarUsuarioPorTokenAtivacao
        {
            TokenAtivacao = tokenAtivacao.Valor
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

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var request = new RequestPegarUsuarioPorTokenAtivacao
        {
            TokenAtivacao = tokenAtivacao.Valor
        };

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_ATIVACAO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Token_Expirado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenAtivacao: tokenAtivacao, false);

        var request = new RequestPegarUsuarioPorTokenAtivacao
        {
            TokenAtivacao = tokenAtivacao.Valor
        };

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_ATIVACAO_EXPIRADO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Token_Vazio(string? token)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenAtivacao: tokenAtivacao, false);

        var request = new RequestPegarUsuarioPorTokenAtivacao
        {
            TokenAtivacao = token
        };

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_ATIVACAO_VAZIO));
    }

    private static PegarUsuarioPorTokenAtivacaoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.TokenAtivacao? tokenAtivacao = null,
        bool tokenValido = false)
    {
        var tokenAtivacaoRepository = new TokenAtivacaoRepositoryBuilder();
        var geradorTokenAtivacao = new GeradorTokenAtivacaoBuilder();

        if (tokenAtivacao != null)
        {
            tokenAtivacaoRepository.SetupPegarTokenAtivacaoPorTokenReturnsTokenAtivacao(tokenAtivacao.Valor, tokenAtivacao, cancellationToken);
            if (tokenValido)
                geradorTokenAtivacao.SetupTokenValidoReturnsTrue(tokenAtivacao);
        }

        return new PegarUsuarioPorTokenAtivacaoUseCase(
            tokenAtivacaoRepository.Build(),
            geradorTokenAtivacao.Build());

    }
}
