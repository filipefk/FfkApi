using FfkApi.Application.UseCases.Feed.Alterar;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.Feed.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarFeedUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var feedAlteracao = FeedBuilder.Build();
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, feedAlteracao: feedAlteracao, organizacaoFeed: organizacaoFeed, equipesFeed: [equipeFeed], usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Feed_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacaoFeed: organizacaoFeed, equipesFeed: [equipeFeed], usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.FEED_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        request.VisibilidadeUsuarios = [];
        request.VisibilidadeEquipes = [];

        var feedAlteracao = FeedBuilder.Build();
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, feedAlteracao: feedAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Nomes_Equipes_Nao_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        var nomeEquipe = "Equipe Inexistente";

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [nomeEquipe];

        var feedAlteracao = FeedBuilder.Build();
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, feedAlteracao: feedAlteracao, organizacaoFeed: organizacaoFeed, usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOMES_DE_EQUIPES_NAO_ENCONTRADOS.Replace("{lista}", nomeEquipe)));
    }

    [Test]
    public async Task Erro_Emails_De_Usuarios_Nao_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);

        var emailUsuario = "email.inexistente@dominio.com";

        request.VisibilidadeUsuarios = [emailUsuario];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var feedAlteracao = FeedBuilder.Build();
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, feedAlteracao: feedAlteracao, organizacaoFeed: organizacaoFeed, equipesFeed: [equipeFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAILS_DE_USUARIOS_NAO_ENCONTRADOS.Replace("{lista}", emailUsuario)));
    }

    [Test]
    public async Task Erro_Email_Usuario_Vazio()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [""];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var feedAlteracao = FeedBuilder.Build();
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, feedAlteracao: feedAlteracao, organizacaoFeed: organizacaoFeed, equipesFeed: [equipeFeed], usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAIL_USUARIO_VAZIO));
    }

    private static AlterarFeedUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Feed? feedAlteracao = null,
        FfkApi.Domain.Entities.Organizacao? organizacaoFeed = null,
        List<FfkApi.Domain.Entities.Equipe>? equipesFeed = null,
        List<FfkApi.Domain.Entities.Usuario>? usuariosFeed = null)
    {
        var feedRepository = new FeedRepositoryBuilder();
        if (feedAlteracao != null)
        {
            feedRepository.SetupExisteFeedComIdReturnsTrue(feedAlteracao.Id, cancellationToken);
            feedRepository.SetupPegarFeedPorIdReturnsFeed(feedAlteracao, cancellationToken);
        }

        var organizacaoRepository = new OrganizacaoRepositoryBuilder();
        if (organizacaoFeed != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(organizacaoFeed.Nome, cancellationToken);
            organizacaoRepository.SetupPegarOrganizacaoPorNomeReturnsOrganizacao(organizacaoFeed, cancellationToken);
        }

        var equipeRepository = new EquipeRepositoryBuilder();
        if (equipesFeed != null && organizacaoFeed != null)
        {
            equipeRepository.SetupPegarPorNomesNaOrganizacaoReturnsEquipes(equipesFeed, organizacaoFeed.Nome, cancellationToken);
        }

        var usuarioRepository = new UsuarioRepositoryBuilder();
        if (usuariosFeed != null && organizacaoFeed != null)
        {
            usuarioRepository.SetupPegarUsuariosAptosPorEmailsReturnsUsuarios(usuariosFeed, organizacaoFeed.Nome, cancellationToken);
        }

        return new AlterarFeedUseCase(
            feedRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            organizacaoRepository.Build(),
            equipeRepository.Build(),
            usuarioRepository.Build());
    }
}
