using FfkApi.Application.UseCases.Feed.Pegar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Feed.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarFeedUseCaseTest
{
    private static void AssertResponseComFeed(ResponseDadosFeed? response, FfkApi.Domain.Entities.Feed feed)
    {
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Id, Is.Not.Null);
        Assert.That(response.Id, Is.EqualTo(feed.Id));
        Assert.That(response.Nome, Is.EqualTo(feed.Nome));
        Assert.That(response.Descricao, Is.EqualTo(feed.Descricao));
        Assert.That(response.PalavrasChave, Is.EqualTo(feed.PalavrasChave));
        Assert.That(response.Status, Is.EqualTo(feed.Status.ToString()));
        Assert.That(response.VisibilidadeUsuarios, Is.EquivalentTo(feed.VisibilidadeUsuarios!.Select(usuario => usuario.Email)));
        Assert.That(response.VisibilidadeEquipes, Is.EquivalentTo(feed.VisibilidadeEquipes!.Select(equipe => equipe.Nome)));
        Assert.That(response.ExpiraEm, Is.EqualTo(feed.ExpiraEm!.Value.ToString("dd/MM/yyyy")));
        Assert.That(response.Organizacao, Is.EqualTo(feed.Organizacao.Nome));
    }

    [Test]
    public async Task Sucesso_Administrador_Pegando_Feed_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var feed = FeedBuilder.Build();

        var request = new RequestPegarFeed
        {
            Id = feed.Id.ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feed: feed);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComFeed(response, feed);
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Pegando_Feed_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var feed = FeedBuilder.Build();

        var request = new RequestPegarFeed
        {
            Id = feed.Id.ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build(organizacao: feed.Organizacao);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feed: feed);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComFeed(response, feed);
    }

    [Test]
    public async Task Erro_Nao_Administrador_Pegando_Feed_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var feed = FeedBuilder.Build();

        var request = new RequestPegarFeed
        {
            Id = feed.Id.ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build();

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feed: feed);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.FEED_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Feed_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestPegarFeed
        {
            Id = Guid.NewGuid().ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado);

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

        var request = new RequestPegarFeed
        {
            Id = id
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feed: feed);

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

        var request = new RequestPegarFeed
        {
            Id = id
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feed: feed);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    private static PegarFeedUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Feed? feed = null)
    {
        var feedRepository = new FeedRepositoryBuilder();

        if (feed != null)
        {
            feedRepository.SetupPegarFeedPorIdReturnsFeed(feed, cancellationToken);
            feedRepository.SetupPegarFeedPorIdReturnsFeed(feed, feed.Organizacao.Id, cancellationToken);
        }

        return new PegarFeedUseCase(
            feedRepository.Build(),
            MapperBuilder.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken));
    }
}
