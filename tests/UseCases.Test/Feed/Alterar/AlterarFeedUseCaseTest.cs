using FfkApi.Application.UseCases.Feed.Alterar;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Feed.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarFeedUseCaseTest
{
    [Test]
    public async Task Sucesso_Administrador_Alterando_Feed_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var feedAlteracao = FeedBuilder.Build(organizacao: organizacaoFeed);
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Administrador_Alterando_A_Organizacao_De_Um_Feed()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoRequest = OrganizacaoBuilder.Build(nome: request.Organizacao);
        var organizacaoFeed = OrganizacaoBuilder.Build(nome: "Organização do Feed");
        var feedAlteracao = FeedBuilder.Build(organizacao: organizacaoFeed);
        feedAlteracao.Id = Guid.Parse(request.Id!);

        request.VisibilidadeUsuarios = [];
        request.VisibilidadeEquipes = [];

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoRequest);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Alterando_Um_Feed_Da_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var feedAlteracao = FeedBuilder.Build(organizacao: organizacaoFeed);
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var usuarioLogado = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Nao_Informando_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();
        request.Organizacao = null;

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var feedAlteracao = FeedBuilder.Build(organizacao: organizacaoFeed);
        feedAlteracao.Id = Guid.Parse(request.Id!);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var usuarioLogado = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Nao_Administrador_Alterando_Feed_De_Outra_Organizacao()
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

        var usuarioLogado = UsuarioBuilder.Build();

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(2));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.FEED_NAO_ENCONTRADO));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Administrador_Alterando_A_Organizacao_De_Um_Feed_Que_Tem_Usuarios()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var organizacaoRequest = OrganizacaoBuilder.Build(nome: request.Organizacao);
        var organizacaoFeed = OrganizacaoBuilder.Build(nome: "Organização do Feed");
        var feedAlteracao = FeedBuilder.Build(organizacao: organizacaoFeed);
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [];

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoRequest);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(2));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.IMPOSSIVEL_TROCAR_ORGANIZACAO_FEED_QUANDO_TEM_VISIBILIDADE_USUARIOS));

        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAILS_DE_USUARIOS_NAO_ENCONTRADOS.Replace("{lista}", request.VisibilidadeUsuarios.ListaSepadadaPorVirgula())));
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

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

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

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao);

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

        var feedAlteracao = FeedBuilder.Build(organizacao: organizacaoFeed);
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoFeed,
            usuariosFeed: [usuarioFeed]);

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

        var feedAlteracao = FeedBuilder.Build(organizacao: organizacaoFeed);
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed]);

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

        var feedAlteracao = FeedBuilder.Build(organizacao: organizacaoFeed);
        feedAlteracao.Id = Guid.Parse(request.Id!);

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            feedAlteracao: feedAlteracao,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAIL_USUARIO_VAZIO));
    }

    private static AlterarFeedUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Feed? feedAlteracao = null,
        FfkApi.Domain.Entities.Organizacao? organizacaoFeed = null,
        List<FfkApi.Domain.Entities.Equipe>? equipesFeed = null,
        List<FfkApi.Domain.Entities.Usuario>? usuariosFeed = null)
    {
        var feedRepository = new FeedRepositoryBuilder();
        if (feedAlteracao != null)
        {
            feedRepository.SetupPegarFeedPorIdReturnsFeed(feedAlteracao, cancellationToken);
            feedRepository.SetupPegarFeedPorIdReturnsFeed(feedAlteracao, feedAlteracao.Organizacao.Id, cancellationToken);
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
            usuarioRepository.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken));
    }
}
