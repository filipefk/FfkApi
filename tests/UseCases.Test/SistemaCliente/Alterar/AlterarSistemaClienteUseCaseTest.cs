using FfkApi.Application.UseCases.SistemaCliente.Alterar;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Criptografia;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.SistemaCliente.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarSistemaClienteUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarSistemaClienteBuilder.Build();

        var sistemaClienteAlteracao = SistemaClienteBuilder.Build();
        sistemaClienteAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, sistemaClienteAlteracao: sistemaClienteAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_SistemaCliente_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarSistemaClienteBuilder.Build();

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SISTEMACLIENTE_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_AppId_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarSistemaClienteBuilder.Build();

        var sistemaClienteAlteracao = SistemaClienteBuilder.Build();
        sistemaClienteAlteracao.Id = Guid.Parse(request.Id!);

        var sistemaClienteMesmoAppId = SistemaClienteBuilder.Build();
        sistemaClienteMesmoAppId.AppId = Guid.Parse(request.AppId!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, sistemaClienteAlteracao: sistemaClienteAlteracao, sistemaClienteMesmoAppId: sistemaClienteMesmoAppId);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.APP_ID_JA_EXISTE));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Nome_Vazio(string? nome)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarSistemaClienteBuilder.Build();
        request.Nome = nome;

        var sistemaClienteAlteracao = SistemaClienteBuilder.Build();
        sistemaClienteAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, sistemaClienteAlteracao: sistemaClienteAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOME_VAZIO));
    }


    private static AlterarSistemaClienteUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.SistemaCliente? sistemaClienteAlteracao = null,
        FfkApi.Domain.Entities.SistemaCliente? sistemaClienteMesmoAppId = null)
    {
        var sistemaClienteRepository = new SistemaClienteRepositoryBuilder();

        if (sistemaClienteAlteracao != null)
        {
            sistemaClienteRepository.SetupPegarSistemaClientePorIdReturnsSistemaCliente(sistemaClienteAlteracao, cancellationToken);
        }

        if (sistemaClienteMesmoAppId != null)
        {
            sistemaClienteRepository.SetupExisteSistemaClienteComAppIdReturnsTrue(sistemaClienteMesmoAppId, cancellationToken);
        }

        return new AlterarSistemaClienteUseCase(
            sistemaClienteRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            EncriptadorSenhaBuilder.Build());
    }
}
