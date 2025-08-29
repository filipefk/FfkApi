using FfkApi.Application.UseCases.Organizacao.Excluir;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;

namespace UnidadeUseCases.Test.Organizacao.Excluir;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ExcluirOrganizacaoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacao = OrganizacaoBuilder.Build();

        var request = new RequestExcluirOrganizacao
        {
            Id = organizacao.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestExcluirOrganizacao
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
    public async Task Erro_Organizacao_Ja_Vinculada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacao = OrganizacaoBuilder.Build();

        var request = new RequestExcluirOrganizacao
        {
            Id = organizacao.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, true);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_JA_VINCULADA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacao = OrganizacaoBuilder.Build();

        var request = new RequestExcluirOrganizacao
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

        var request = new RequestExcluirOrganizacao
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

    private static ExcluirOrganizacaoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Organizacao? organizacao = null,
        bool organizacaoJaVinculada = false)
    {
        var organizacaoRepository = new OrganizacaoRepositoryBuilder();

        if (organizacao != null)
        {
            organizacaoRepository.SetupPegarOrganizacaoPorIdReturnsOrganizacao(organizacao, cancellationToken);
            if (organizacaoJaVinculada)
            {
                organizacaoRepository.SetupExcluirThrowsInvalidOperationException(organizacao.Id, cancellationToken);
            }
        }

        return new ExcluirOrganizacaoUseCase(
            organizacaoRepository.Build(),
            UnitOfWorkBuilder.Build());
    }
}
