using FfkApi.Application.UseCases.Login.LoginUsuario;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Criptografia;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.Login.LoginUsuario;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LoginUsuarioUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestLoginUsuarioBuilder.Build(usuario);

        usuario.Senha = EncriptadorSenhaBuilder.Build().Encriptar(usuario.Senha!);

        var useCase = CriarUseCase(usuario, cancellationToken);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(usuario.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Tokens.AccessToken));
        Assert.That(!string.IsNullOrWhiteSpace(response.Tokens.RefreshToken));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestLoginUsuarioBuilder.Build(usuario);

        usuario.Senha = EncriptadorSenhaBuilder.Build().Encriptar(usuario.Senha!);

        var useCase = CriarUseCase();

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<InvalidLoginUsuarioException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAIL_OU_SENHA_INVALIDOS));
    }

    [Test]
    public async Task Erro_Senha_Invalida()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();

        var request = RequestLoginUsuarioBuilder.Build(usuario);

        usuario.Senha = EncriptadorSenhaBuilder.Build().Encriptar(usuario.Senha!);

        request.Senha = "SenhaInvalida";

        var useCase = CriarUseCase(usuario, cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<InvalidLoginUsuarioException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAIL_OU_SENHA_INVALIDOS));
    }

    private static LoginUsuarioUseCase CriarUseCase(
        FfkApi.Domain.Entities.Usuario? usuario = null,
        CancellationToken cancellationToken = default)
    {
        var usuarioRepository = new UsuarioRepositoryBuilder();

        if (usuario != null)
            usuarioRepository.SetupPegarUsuarioAptoPorEmailReturnsUsuario(usuario, cancellationToken);

        return new LoginUsuarioUseCase(
            usuarioRepository.Build(),
            EncriptadorSenhaBuilder.Build(),
            GeradorTokenUsuarioBuilder.Build(),
            new GeradorRefreshTokenBuilder().Build(),
            new RefreshTokenRepositoryBuilder().Build(),
            UnitOfWorkBuilder.Build());
    }
}
