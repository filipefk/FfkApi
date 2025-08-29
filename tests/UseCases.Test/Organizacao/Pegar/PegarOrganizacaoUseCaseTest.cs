using FfkApi.Application.UseCases.Organizacao.Pegar;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;

namespace UnidadeUseCases.Test.Organizacao.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarOrganizacaoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacao = OrganizacaoBuilder.Build();

        var request = new RequestPegarOrganizacao
        {
            Id = organizacao.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);
        Assert.That(response.Id, Is.EqualTo(organizacao.Id));
        Assert.That(response.Nome, Is.EqualTo(organizacao.Nome));
        Assert.That(response.Descricao, Is.EqualTo(organizacao.Descricao));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestPegarOrganizacao
        {
            Id = Guid.NewGuid().ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacao = OrganizacaoBuilder.Build();

        var request = new RequestPegarOrganizacao
        {
            Id = id
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao);

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

        var organizacao = OrganizacaoBuilder.Build();

        var request = new RequestPegarOrganizacao
        {
            Id = id
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    private static PegarOrganizacaoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Organizacao? organizacao = null)
    {
        var organizacaoRepository = new OrganizacaoRepositoryBuilder();

        if (organizacao != null)
        {
            organizacaoRepository.SetupPegarOrganizacaoPorIdReturnsOrganizacao(organizacao, cancellationToken);
        }

        return new PegarOrganizacaoUseCase(
            organizacaoRepository.Build(),
            MapperBuilder.Build());
    }
}
