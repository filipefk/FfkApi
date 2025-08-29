using FfkApi.Application.UseCases.Anexo.Pegar;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;

namespace UnidadeUseCases.Test.Anexo.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarDadosAnexoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();

        var request = new RequestPegarDadosAnexo
        {
            Id = anexo.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);

        Assert.That(response.Nome, Is.EqualTo(anexo.Nome));
        Assert.That(response.Descricao, Is.EqualTo(anexo.Descricao));
        Assert.That(response.NomeArquivo, Is.EqualTo(anexo.NomeArquivo));
        Assert.That(response.TamanhoBytes, Is.EqualTo(anexo.TamanhoBytes));
    }

    [Test]
    public async Task Erro_Anexo_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestPegarDadosAnexo
        {
            Id = Guid.NewGuid().ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ANEXO_NAO_ENCONTRADO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();

        var request = new RequestPegarDadosAnexo
        {
            Id = id
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo);

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

        var anexo = AnexoBuilder.Build();

        var request = new RequestPegarDadosAnexo
        {
            Id = id
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    private static PegarDadosAnexoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Anexo? anexo = null)
    {
        var anexoRepository = new AnexoRepositoryBuilder();

        if (anexo != null)
        {
            anexoRepository.SetupPegarAnexoPorIdReturnsAnexo(anexo, cancellationToken);
        }

        return new PegarDadosAnexoUseCase(
            anexoRepository.Build(),
            MapperBuilder.Build());
    }
}
