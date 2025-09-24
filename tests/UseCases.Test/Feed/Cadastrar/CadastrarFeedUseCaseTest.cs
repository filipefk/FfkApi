using FfkApi.Application.UseCases.Feed.Cadastrar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Feed.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarFeedUseCaseTest
{
    private static void AssertResponseComRequest(ResponseDadosFeed? response, RequestCadastrarFeed request)
    {
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Id, Is.Not.Null);
        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));
        Assert.That(response.PalavrasChave, Is.EqualTo(request.PalavrasChave));
        Assert.That(response.Status, Is.EqualTo(request.Status));
        Assert.That(response.VisibilidadeUsuarios, Is.EquivalentTo(request.VisibilidadeUsuarios!));
        Assert.That(response.VisibilidadeEquipes, Is.EquivalentTo(request.VisibilidadeEquipes!));
        Assert.That(response.ExpiraEm, Is.EqualTo(request.ExpiraEm));
        Assert.That(response.Organizacao, Is.EqualTo(request.Organizacao));
    }

    [Test]
    public async Task Sucesso_Administrador_Cadastrando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedBuilder.Build();

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComRequest(request: request, response: response);
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Informando_A_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Feeds"], organizacao: organizacaoFeed);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComRequest(request: request, response: response);
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Nao_Informando_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = null;

        var organizacaoFeed = OrganizacaoBuilder.Build();
        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Feeds"], organizacao: organizacaoFeed);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoFeed: organizacaoFeed,
            equipesFeed: [equipeFeed],
            usuariosFeed: [usuarioFeed]);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComRequest(request: request, response: response);
    }

    [Test]
    public async Task Erro_Nao_Administrador_Cadastrando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Feeds"]);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

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
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Administrador_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedBuilder.Build();

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        request.VisibilidadeUsuarios = [];
        request.VisibilidadeEquipes = [];

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado);

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

        var request = RequestCadastrarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Feeds"], organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        var nomeEquipe = "Equipe Inexistente";

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [nomeEquipe];

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, organizacaoFeed: organizacaoFeed, usuariosFeed: [usuarioFeed]);

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

        var request = RequestCadastrarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Feeds"], organizacao: organizacaoFeed);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);

        var emailUsuario = "email.inexistente@dominio.com";

        request.VisibilidadeUsuarios = [emailUsuario];
        request.VisibilidadeEquipes = [equipeFeed.Nome];

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
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
    public async Task Erro_Nome_Equipe_Vazia()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedBuilder.Build();

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Feeds"], organizacao: organizacaoFeed);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        request.VisibilidadeUsuarios = [usuarioFeed.Email];
        request.VisibilidadeEquipes = [equipeFeed.Nome, ""];

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, organizacaoFeed: organizacaoFeed, equipesFeed: [equipeFeed], usuariosFeed: [usuarioFeed]);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOME_EQUIPE_VAZIA));
    }

    private static CadastrarFeedUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Organizacao? organizacaoFeed = null,
        List<FfkApi.Domain.Entities.Equipe>? equipesFeed = null,
        List<FfkApi.Domain.Entities.Usuario>? usuariosFeed = null)
    {
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

        return new CadastrarFeedUseCase(
            new FeedRepositoryBuilder().Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            organizacaoRepository.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken),
            equipeRepository.Build(),
            usuarioRepository.Build());
    }
}