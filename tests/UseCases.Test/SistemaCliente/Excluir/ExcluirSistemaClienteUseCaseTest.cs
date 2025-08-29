using FfkApi.Application.UseCases.SistemaCliente.Excluir;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;

namespace UnidadeUseCases.Test.SistemaCliente.Excluir;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ExcluirSistemaClienteUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = SistemaClienteBuilder.Build();

        var request = new RequestExcluirSistemaCliente
        {
            Id = sistemaCliente.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, sistemaCliente: sistemaCliente);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_SistemaCliente_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestExcluirSistemaCliente
        {
            Id = Guid.NewGuid().ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SISTEMACLIENTE_NAO_ENCONTRADO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = SistemaClienteBuilder.Build();

        var request = new RequestExcluirSistemaCliente
        {
            Id = id
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, sistemaCliente: sistemaCliente);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_VAZIO));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = SistemaClienteBuilder.Build();

        var request = new RequestExcluirSistemaCliente
        {
            Id = id
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, sistemaCliente: sistemaCliente);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    private static ExcluirSistemaClienteUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.SistemaCliente? sistemaCliente = null)
    {
        var sistemaClienteRepository = new SistemaClienteRepositoryBuilder();

        if (sistemaCliente != null)
        {
            sistemaClienteRepository.SetupPegarSistemaClientePorIdReturnsSistemaCliente(sistemaCliente, cancellationToken);
        }

        return new ExcluirSistemaClienteUseCase(
            sistemaClienteRepository.Build(),
            UnitOfWorkBuilder.Build());
    }
}
