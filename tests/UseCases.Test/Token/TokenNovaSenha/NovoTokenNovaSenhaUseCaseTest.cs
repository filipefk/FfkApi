using FfkApi.Application.UseCases.Token.TokenNovaSenha;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.Token.TokenNovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class NovoTokenNovaSenhaUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestNovoTokenNovaSenhaBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuario: usuario);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(usuario.Nome));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Nome_Vazio(string? nome)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestNovoTokenNovaSenhaBuilder.Build(usuario);
        request.Nome = nome;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuario: usuario);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOME_VAZIO));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestNovoTokenNovaSenhaBuilder.Build(usuario);

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Dados_Invalidos()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestNovoTokenNovaSenhaBuilder.Build(usuario);
        request.Cpf = "75242237651";

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuario: usuario);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.DADOS_INVALIDOS));
    }

    private static NovoTokenNovaSenhaUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario? usuario = null)
    {
        var usuarioRepository = new UsuarioRepositoryBuilder();

        if (usuario != null)
            usuarioRepository.SetupPegarUsuarioAptoPorEmailReturnsUsuario(usuario, cancellationToken);

        return new NovoTokenNovaSenhaUseCase(
            new TokenNovaSenhaRepositoryBuilder().Build(),
            new GeradorTokenNovaSenhaBuilder().Build(),
            usuarioRepository.Build(),
            UnitOfWorkBuilder.Build());

    }
}
