using FfkApi.Application.UseCases.Feed.Excluir;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Feed.Excluir;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ExcluirFeedUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var feed = FeedBuilder.Build();

        var request = new RequestExcluirFeed
        {
            Id = feed.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, feed: feed);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Feed_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestExcluirFeed
        {
            Id = Guid.NewGuid().ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.FEED_NAO_ENCONTRADO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var feed = FeedBuilder.Build();

        var request = new RequestExcluirFeed
        {
            Id = id
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, feed: feed);

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

        var feed = FeedBuilder.Build();

        var request = new RequestExcluirFeed
        {
            Id = id
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, feed: feed);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    private static ExcluirFeedUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Feed? feed = null)
    {
        var feedRepository = new FeedRepositoryBuilder();

        if (feed != null)
        {
            feedRepository.SetupPegarFeedPorIdReturnsFeed(feed, cancellationToken);
        }

        return new ExcluirFeedUseCase(
            feedRepository.Build(),
            UnitOfWorkBuilder.Build(),
            new ArmazenadorDeAnexoServiceBuilder().Build());
    }
}
