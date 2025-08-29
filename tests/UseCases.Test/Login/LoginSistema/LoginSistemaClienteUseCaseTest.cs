using FfkApi.Application.UseCases.Login.LoginSistema;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Criptografia;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.Login.LoginSistema;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LoginSistemaClienteUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = SistemaClienteBuilder.Build();

        var request = RequestLoginSistemaClienteBuilder.Build(sistemaCliente);

        sistemaCliente.Senha = EncriptadorSenhaBuilder.Build().Encriptar(sistemaCliente.Senha!);

        var useCase = CriarUseCase(sistemaCliente, cancellationToken);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(sistemaCliente.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.AccessToken));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = SistemaClienteBuilder.Build();

        var request = RequestLoginSistemaClienteBuilder.Build(sistemaCliente);

        sistemaCliente.Senha = EncriptadorSenhaBuilder.Build().Encriptar(sistemaCliente.Senha!);

        var useCase = CriarUseCase();

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<InvalidLoginSistemaClienteException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.APP_ID_OU_SENHA_INVALIDOS));
    }

    [Test]
    public async Task Erro_Senha_Invalida()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = SistemaClienteBuilder.Build();

        var request = RequestLoginSistemaClienteBuilder.Build(sistemaCliente);

        sistemaCliente.Senha = EncriptadorSenhaBuilder.Build().Encriptar(sistemaCliente.Senha!);

        request.Senha = "SenhaInvalida";

        var useCase = CriarUseCase(sistemaCliente, cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<InvalidLoginSistemaClienteException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.APP_ID_OU_SENHA_INVALIDOS));
    }

    private static LoginSistemaClienteUseCase CriarUseCase(
        FfkApi.Domain.Entities.SistemaCliente? sistemaCliente = null,
        CancellationToken cancellationToken = default)
    {
        var sistemaClienteRepository = new SistemaClienteRepositoryBuilder();

        if (sistemaCliente != null)
            sistemaClienteRepository.SetupPegarSistemaClienteAtivoPorAppIdReturnsSistemaCliente(sistemaCliente, cancellationToken);

        return new LoginSistemaClienteUseCase(
            sistemaClienteRepository.Build(),
            EncriptadorSenhaBuilder.Build(),
            GeradorTokenSistemaClienteBuilder.Build());
    }
}
