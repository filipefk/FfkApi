using FfkApi.Application.UseCases.Usuario.NovaSenha;
using FfkApi.Domain.Entities;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Criptografia;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.Usuario.NovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class NovaSenhaUsuarioUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenNovaSenha = TokenNovaSenhaBuilder.Build(usuario);

        var request = RequestNovaSenhaUsuarioBuilder.Build(usuario, tokenNovaSenha.Valor);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenNovaSenha: tokenNovaSenha, true);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }


    [Test]
    public async Task Erro_Token_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenNovaSenha = TokenNovaSenhaBuilder.Build(usuario);

        var request = RequestNovaSenhaUsuarioBuilder.Build(usuario, tokenNovaSenha.Valor);

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

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

        var request = RequestNovaSenhaUsuarioBuilder.Build(usuario, tokenNovaSenha.Valor);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenNovaSenha: tokenNovaSenha, false);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_NOVA_SENHA_EXPIRADO));
    }

    [Test]
    public async Task Erro_Dados_Invalidos()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var tokenNovaSenha = TokenNovaSenhaBuilder.Build(usuario);

        var request = RequestNovaSenhaUsuarioBuilder.Build(usuario, tokenNovaSenha.Valor);
        request.Email = "emailerrado@gmail.com";

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenNovaSenha: tokenNovaSenha, true);

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

        var tokenNovaSenha = TokenNovaSenhaBuilder.Build(usuario);

        var request = RequestNovaSenhaUsuarioBuilder.Build(usuario, tokenNovaSenha.Valor);
        request.NovaSenha = "senhainvalida";

        var useCase = CriarUseCase(cancellationToken: cancellationToken, tokenNovaSenha: tokenNovaSenha, true);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SENHA_INVALIDA));
    }


    public static NovaSenhaUsuarioUseCase CriarUseCase(
        CancellationToken cancellationToken,
        TokenNovaSenha? tokenNovaSenha = null,
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

        return new NovaSenhaUsuarioUseCase(
            new UsuarioRepositoryBuilder().Build(),
            tokenNovaSenhaRepository.Build(),
            UnitOfWorkBuilder.Build(),
            EncriptadorSenhaBuilder.Build(),
            geradorTokenNovaSenha.Build());
    }
}
