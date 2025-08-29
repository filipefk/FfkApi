using FfkApi.Application.UseCases.Usuario.TrocarSenha;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Criptografia;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Usuario.TrocarSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class TrocarSenhaUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestTrocarSenhaBuilder.Build(usuario.Senha!);

        var passwordEncryptor = EncriptadorSenhaBuilder.Build();

        usuario.Senha = passwordEncryptor.Encriptar(request.SenhaAntiga!);

        var useCase = CriarUseCase(cancellationToken, usuario, passwordEncryptor);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Nenhuma_Alteracao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestTrocarSenhaBuilder.Build(usuario.Senha!);
        request.NovaSenha = request.SenhaAntiga;

        var passwordEncryptor = EncriptadorSenhaBuilder.Build();

        usuario.Senha = passwordEncryptor.Encriptar(request.SenhaAntiga!);

        var useCase = CriarUseCase(cancellationToken, usuario, passwordEncryptor);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var exception = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        var mensagensDeErro = exception!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NENHUMA_ALTERACAO));
    }

    [Test]
    public async Task Erro_Senha_Diferente_Da_Senha_Atual()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestTrocarSenhaBuilder.Build();

        var passwordEncryptor = EncriptadorSenhaBuilder.Build();

        var useCase = CriarUseCase(cancellationToken, usuario, passwordEncryptor);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var exception = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        var mensagensDeErro = exception!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SENHA_ANTIGA_DIFERENTE_DA_SENHA_INFORMADA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Senha_Antiga_Vazia(string? senha)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestTrocarSenhaBuilder.Build(usuario.Senha!);

        var passwordEncryptor = EncriptadorSenhaBuilder.Build();

        usuario.Senha = passwordEncryptor.Encriptar(request.SenhaAntiga!);

        request.SenhaAntiga = senha;

        var useCase = CriarUseCase(cancellationToken, usuario, passwordEncryptor);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var exception = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        var mensagensDeErro = exception!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SENHA_VAZIA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Nova_Senha_Vazia(string? senha)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestTrocarSenhaBuilder.Build(usuario.Senha!);
        request.NovaSenha = senha;

        var passwordEncryptor = EncriptadorSenhaBuilder.Build();

        usuario.Senha = passwordEncryptor.Encriptar(request.SenhaAntiga!);

        var useCase = CriarUseCase(cancellationToken, usuario, passwordEncryptor);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var exception = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        var mensagensDeErro = exception!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SENHA_VAZIA));
    }

    [Test]
    public async Task Erro_Nova_Senha_Invalida()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestTrocarSenhaBuilder.Build(usuario.Senha!);
        request.NovaSenha = "SenhaInvalida";

        var passwordEncryptor = EncriptadorSenhaBuilder.Build();

        usuario.Senha = passwordEncryptor.Encriptar(request.SenhaAntiga!);

        var useCase = CriarUseCase(cancellationToken, usuario, passwordEncryptor);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var exception = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        var mensagensDeErro = exception!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SENHA_INVALIDA));
    }

    private static TrocarSenhaUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuario,
        IEncriptadorSenha passwordEncriptor)
    {
        var usuarioRepositoryBuilder = new UsuarioRepositoryBuilder();
        usuarioRepositoryBuilder.SetupPegarUsuarioAptoPorIdReturnsUsuario(usuario, cancellationToken);

        return new TrocarSenhaUseCase(
            UsuarioLogadoServiceBuilder.Build(usuario, cancellationToken),
            passwordEncriptor,
            usuarioRepositoryBuilder.Build(),
            UnitOfWorkBuilder.Build());
    }
}
