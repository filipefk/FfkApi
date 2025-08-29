using FfkApi.Application.UseCases.SistemaCliente.Pegar;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;

namespace UnidadeUseCases.Test.SistemaCliente.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarSistemaClienteUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = SistemaClienteBuilder.Build();

        var request = new RequestPegarSistemaCliente
        {
            Id = sistemaCliente.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, sistemaCliente: sistemaCliente);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);
        Assert.That(response.Id, Is.EqualTo(sistemaCliente.Id));

        Assert.That(response.AppId, Is.Not.Null);
        Assert.That(response.AppId, Is.EqualTo(sistemaCliente.AppId));

        Assert.That(response.Nome, Is.Not.Null);
        Assert.That(response.Nome, Is.EqualTo(sistemaCliente.Nome));

        Assert.That(response.Descricao, Is.Not.Null);
        Assert.That(response.Descricao, Is.EqualTo(sistemaCliente.Descricao));

        Assert.That(response.Status, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo(sistemaCliente.Status.ToString()));
    }

    [Test]
    public async Task Erro_SistemaCliente_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestPegarSistemaCliente
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

        var request = new RequestPegarSistemaCliente
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

        var request = new RequestPegarSistemaCliente
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

    private static PegarSistemaClienteUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.SistemaCliente? sistemaCliente = null)
    {
        var sistemaClienteRepository = new SistemaClienteRepositoryBuilder();

        if (sistemaCliente != null)
        {
            sistemaClienteRepository.SetupPegarSistemaClientePorIdReturnsSistemaCliente(sistemaCliente, cancellationToken);
        }

        return new PegarSistemaClienteUseCase(
            sistemaClienteRepository.Build(),
            MapperBuilder.Build());
    }
}
