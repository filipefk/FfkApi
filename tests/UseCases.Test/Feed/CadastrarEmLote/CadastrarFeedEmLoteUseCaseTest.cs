using FfkApi.Application.UseCases.Feed.CadastrarEmLote;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.Feed.CadastrarEmLote;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarFeedEmLoteUseCaseTest
{
    [Test]
    public async Task Sucesso_Total()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedEmLoteBuilder.Build(4);

        var nomeOrganizacao = request.Feeds[0].Organizacao!;

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: nomeOrganizacao);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        foreach (var feed in request.Feeds)
        {
            feed.Organizacao = nomeOrganizacao;
        }

        request.Feeds[0].VisibilidadeUsuarios = [usuarioFeed.Email];
        request.Feeds[0].VisibilidadeEquipes = [equipeFeed.Nome];

        request.Feeds[1].VisibilidadeUsuarios = [];
        request.Feeds[1].VisibilidadeEquipes = [equipeFeed.Nome];

        request.Feeds[2].VisibilidadeUsuarios = [usuarioFeed.Email];
        request.Feeds[2].VisibilidadeEquipes = [];

        for (int i = 3; i < request.Feeds.Count; i++)
        {
            request.Feeds[i].VisibilidadeUsuarios = [usuarioFeed.Email];
            request.Feeds[i].VisibilidadeEquipes = [equipeFeed.Nome];
        }

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacaoFeed: organizacaoFeed, equipeFeed: equipeFeed, usuarioFeed: usuarioFeed);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados.Count, Is.EqualTo(request.Feeds.Count));

        var cadastradosPorNome = response.Cadastrados
            .ToDictionary(
                c => c.Nome,
                c => c
            );

        foreach (var feedRequest in request.Feeds)
        {
            Assert.That(cadastradosPorNome.ContainsKey(feedRequest.Nome!), $"Feed '{feedRequest.Nome}' não encontrada nos cadastrados.");

            var cadastrado = cadastradosPorNome[feedRequest.Nome!];

            Assert.That(cadastrado.Descricao, Is.EqualTo(feedRequest.Descricao), $"Descrição divergente para '{feedRequest.Nome}'.");
            Assert.That(cadastrado.PalavrasChave, Is.EqualTo(feedRequest.PalavrasChave), $"Palavras chave divergente para '{feedRequest.Nome}'.");
            Assert.That(cadastrado.Status, Is.EqualTo(feedRequest.Status), $"Status divergente para '{feedRequest.Nome}'.");
            Assert.That(cadastrado.Anexos, Is.Not.Null);
            Assert.That(cadastrado.Anexos, Is.Empty);
            Assert.That(cadastrado.VisibilidadeUsuarios, Is.EquivalentTo(feedRequest.VisibilidadeUsuarios!), $"Visibilidade Usuarios divergente para '{feedRequest.Nome}'.");
            Assert.That(cadastrado.VisibilidadeEquipes, Is.EquivalentTo(feedRequest.VisibilidadeEquipes!), $"Visibilidade Equipes divergente para '{feedRequest.Nome}'.");
            if (string.IsNullOrWhiteSpace(feedRequest.ExpiraEm))
            {
                Assert.That(string.IsNullOrWhiteSpace(cadastrado.ExpiraEm), $"Data de expiração deveria ser nulo ou vazio para '{feedRequest.Nome}'.");
            }
            else
            {
                Assert.That(cadastrado.ExpiraEm, Is.EqualTo(feedRequest.ExpiraEm), $"Data de expiração divergente para '{feedRequest.Nome}'.");
            }
            Assert.That(cadastrado.Organizacao, Is.EqualTo(feedRequest.Organizacao), $"Organizacao divergente para '{feedRequest.Nome}'.");
        }

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros, Is.Empty);

        Assert.That(response.TotalCadastrados, Is.Not.Null);
        Assert.That(response.TotalCadastrados, Is.EqualTo(request.Feeds.Count));

        Assert.That(response.TotalErros, Is.Not.Null);
        Assert.That(response.TotalErros, Is.EqualTo(0));

        Assert.That(response.Resultado, Is.EqualTo(StatusCadastroLote.SucessoTotal.ToString()));
    }

    [Test]
    public async Task Sucesso_Parcial()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedEmLoteBuilder.Build(4);

        var nomeOrganizacao = request.Feeds[0].Organizacao!;

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: nomeOrganizacao);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        foreach (var feed in request.Feeds)
        {
            feed.Organizacao = nomeOrganizacao;
        }

        var nomeEquipe = "Equipe Inexistente";

        request.Feeds[0].VisibilidadeUsuarios = [usuarioFeed.Email];
        request.Feeds[0].VisibilidadeEquipes = [nomeEquipe];

        request.Feeds[1].VisibilidadeUsuarios = [];
        request.Feeds[1].VisibilidadeEquipes = [equipeFeed.Nome];

        request.Feeds[2].VisibilidadeUsuarios = [usuarioFeed.Email];
        request.Feeds[2].VisibilidadeEquipes = [];

        for (int i = 3; i < request.Feeds.Count; i++)
        {
            request.Feeds[i].VisibilidadeUsuarios = [usuarioFeed.Email];
            request.Feeds[i].VisibilidadeEquipes = [equipeFeed.Nome];
        }

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacaoFeed: organizacaoFeed, equipeFeed: equipeFeed, usuarioFeed: usuarioFeed);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados.Count, Is.EqualTo(request.Feeds.Count - 1));

        var cadastradosPorNome = response.Cadastrados
            .ToDictionary(
                c => c.Nome,
                c => c
            );

        for (int i = 1; i < request.Feeds.Count; i++)
        {
            var feedRequest = request.Feeds[i];
            Assert.That(cadastradosPorNome.ContainsKey(feedRequest.Nome!), $"Feed '{feedRequest.Nome}' não encontrada nos cadastrados.");

            var cadastrado = cadastradosPorNome[feedRequest.Nome!];

            Assert.That(cadastrado.Descricao, Is.EqualTo(feedRequest.Descricao), $"Descrição divergente para '{feedRequest.Nome}'.");
            Assert.That(cadastrado.PalavrasChave, Is.EqualTo(feedRequest.PalavrasChave), $"Palavras chave divergente para '{feedRequest.Nome}'.");
            Assert.That(cadastrado.Status, Is.EqualTo(feedRequest.Status), $"Status divergente para '{feedRequest.Nome}'.");
            Assert.That(cadastrado.Anexos, Is.Not.Null);
            Assert.That(cadastrado.Anexos, Is.Empty);
            Assert.That(cadastrado.VisibilidadeUsuarios, Is.EquivalentTo(feedRequest.VisibilidadeUsuarios!), $"Visibilidade Usuarios divergente para '{feedRequest.Nome}'.");
            Assert.That(cadastrado.VisibilidadeEquipes, Is.EquivalentTo(feedRequest.VisibilidadeEquipes!), $"Visibilidade Equipes divergente para '{feedRequest.Nome}'.");
            if (string.IsNullOrWhiteSpace(feedRequest.ExpiraEm))
            {
                Assert.That(string.IsNullOrWhiteSpace(cadastrado.ExpiraEm), $"Data de expiração deveria ser nulo ou vazio para '{feedRequest.Nome}'.");
            }
            else
            {
                Assert.That(cadastrado.ExpiraEm, Is.EqualTo(feedRequest.ExpiraEm), $"Data de expiração divergente para '{feedRequest.Nome}'.");
            }
            Assert.That(cadastrado.Organizacao, Is.EqualTo(feedRequest.Organizacao), $"Organizacao divergente para '{feedRequest.Nome}'.");
        }

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros.Count, Is.EqualTo(1));

        Assert.That(response.Erros.FirstOrDefault()!.MensagensDeErro[0], Is.EqualTo(ResourceMessagesException.NOME_EQUIPE_NAO_ENCONTRADO.Replace("{nome-equipe}", nomeEquipe)));

        Assert.That(response.TotalCadastrados, Is.Not.Null);
        Assert.That(response.TotalCadastrados, Is.EqualTo(request.Feeds.Count - 1));

        Assert.That(response.TotalErros, Is.Not.Null);
        Assert.That(response.TotalErros, Is.EqualTo(1));

        Assert.That(response.Resultado, Is.EqualTo(StatusCadastroLote.SucessoParcial.ToString()));
    }

    [Test]
    public async Task Falha_Total()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedEmLoteBuilder.Build(2);

        var nomeOrganizacao = request.Feeds[0].Organizacao!;

        var organizacaoFeed = OrganizacaoBuilder.Build(nome: nomeOrganizacao);
        var equipeFeed = EquipeBuilder.Build(organizacao: organizacaoFeed);
        var usuarioFeed = UsuarioBuilder.Build(organizacao: organizacaoFeed);

        foreach (var feed in request.Feeds)
        {
            feed.Organizacao = nomeOrganizacao;
            feed.VisibilidadeUsuarios = [];
            feed.VisibilidadeEquipes = [];
        }

        var requestNomeNull = request.Feeds.FirstOrDefault();
        requestNomeNull!.Nome = null;

        var requestDescricaoNull = request.Feeds.Skip(1).FirstOrDefault();
        requestDescricaoNull!.Descricao = null;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacaoFeed: organizacaoFeed, equipeFeed: equipeFeed, usuarioFeed: usuarioFeed);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Empty);

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros.Count, Is.EqualTo(request.Feeds.Count));

        var responseErroQueRequestTemNomeNull = response.Erros.FirstOrDefault(e => string.IsNullOrEmpty(e.Request.Nome));

        var responseRequestQueTemNomeNull = responseErroQueRequestTemNomeNull!.Request;
        Assert.That(responseRequestQueTemNomeNull.Descricao, Is.EqualTo(requestNomeNull!.Descricao));

        var mensagensDeErroNomeNull = responseErroQueRequestTemNomeNull.MensagensDeErro;
        Assert.That(mensagensDeErroNomeNull.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErroNomeNull.FirstOrDefault()!, Is.EqualTo(ResourceMessagesException.NOME_VAZIO));

        var responseErroQueRequestTemDescricaoNull = response.Erros.FirstOrDefault(e => string.IsNullOrEmpty(e.Request.Descricao));

        var responseRequestQueTemDescricaoNull = responseErroQueRequestTemDescricaoNull!.Request;
        Assert.That(responseRequestQueTemDescricaoNull.Nome, Is.EqualTo(requestDescricaoNull!.Nome));

        var mensagensDeErroDescricaoNull = responseErroQueRequestTemDescricaoNull.MensagensDeErro;
        Assert.That(mensagensDeErroDescricaoNull.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErroDescricaoNull.FirstOrDefault()!, Is.EqualTo(ResourceMessagesException.DESCRICAO_VAZIA));

        Assert.That(response.TotalCadastrados, Is.Not.Null);
        Assert.That(response.TotalCadastrados, Is.EqualTo(0));

        Assert.That(response.TotalErros, Is.Not.Null);
        Assert.That(response.TotalErros, Is.EqualTo(request.Feeds.Count));

        Assert.That(response.Resultado, Is.EqualTo(StatusCadastroLote.Falha.ToString()));
    }

    [Test]
    public async Task Erro_Lista_Feeds_Vazia()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestCadastrarFeedEmLote
        {
            Feeds = []
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Not.Null);
        Assert.That(response.Cadastrados, Is.Empty);

        Assert.That(response.Erros, Is.Not.Null);
        Assert.That(response.Erros.Count, Is.EqualTo(1));
        Assert.That(response.Erros.FirstOrDefault()!.MensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(response.Erros.FirstOrDefault()!.MensagensDeErro, Contains.Item(ResourceMessagesException.LISTA_DE_FEED_VAZIA));
        Assert.That(response.Erros.FirstOrDefault()!.Request, Is.Null);
    }


    private static CadastrarFeedEmLoteUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Organizacao? organizacaoFeed = null,
        FfkApi.Domain.Entities.Equipe? equipeFeed = null,
        FfkApi.Domain.Entities.Usuario? usuarioFeed = null)
    {
        var organizacaoRepository = new OrganizacaoRepositoryBuilder();
        if (organizacaoFeed != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(organizacaoFeed.Nome, cancellationToken);
            organizacaoRepository.SetupPegarOrganizacaoPorNomeReturnsOrganizacao(organizacaoFeed, cancellationToken);
        }

        var equipeRepository = new EquipeRepositoryBuilder();
        if (equipeFeed != null)
        {
            equipeRepository.SetupPegarEquipePorNomeReturnsEquipe(equipeFeed, cancellationToken);
            if (organizacaoFeed != null && equipeFeed.Organizacao.Nome == organizacaoFeed.Nome)
            {
                equipeRepository.SetupExisteEquipeComNomeReturnsTrue(equipeFeed.Nome, organizacaoFeed.Nome, cancellationToken);
            }
        }

        var usuarioRepository = new UsuarioRepositoryBuilder();
        if (usuarioFeed != null)
        {
            usuarioRepository.SetupPegarUsuarioAptoPorEmailReturnsUsuario(usuarioFeed, cancellationToken);
            if (organizacaoFeed != null && usuarioFeed.Organizacao.Nome == organizacaoFeed.Nome)
            {
                usuarioRepository.SetupExisteUsuarioAptoComEmailNaOrganizacaoReturnsTrue(usuarioFeed.Email, organizacaoFeed.Nome, cancellationToken);
            }
        }

        return new CadastrarFeedEmLoteUseCase(
            new FeedRepositoryBuilder().Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            organizacaoRepository.Build(),
            equipeRepository.Build(),
            usuarioRepository.Build());
    }
}
