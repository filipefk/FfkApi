using FfkApi.Application.UseCases.Usuario.Ativar;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Criptografia;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.Usuario.Ativar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AtivarUsuarioUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var request = RequestAtivarUsuarioBuilder.Build(usuario, tokenAtivacao.Valor);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenAtivacao: tokenAtivacao, true);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Token_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var request = RequestAtivarUsuarioBuilder.Build(usuario, tokenAtivacao.Valor);

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

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

        var request = RequestAtivarUsuarioBuilder.Build(usuario, tokenAtivacao.Valor);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenAtivacao: tokenAtivacao, false);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_ATIVACAO_EXPIRADO));
    }

    [Test]
    public async Task Erro_Dados_Invalidos()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var request = RequestAtivarUsuarioBuilder.Build(usuario, tokenAtivacao.Valor);
        request.Email = "emailerrado@gmail.com";

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenAtivacao: tokenAtivacao, true);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.DADOS_INVALIDOS));
    }

    [Test]
    public async Task Erro_Senha_Invalida()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var request = RequestAtivarUsuarioBuilder.Build(usuario, tokenAtivacao.Valor);
        request.Senha = "senhainvalida";

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenAtivacao: tokenAtivacao, true);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SENHA_INVALIDA));
    }

    private static AtivarUsuarioUseCase CriarUseCase(
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

        return new AtivarUsuarioUseCase(
            new UsuarioRepositoryBuilder().Build(),
            tokenAtivacaoRepository.Build(),
            UnitOfWorkBuilder.Build(),
            EncriptadorSenhaBuilder.Build(),
            geradorTokenAtivacao.Build());
    }
}
