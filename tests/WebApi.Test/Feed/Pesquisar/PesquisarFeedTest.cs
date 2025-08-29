using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Integracao.Test.Feed.Pesquisar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PesquisarFeedTest : FfkApiClassFixture
{
    private readonly string _baseUrlPesquisar = "feed/pesquisar";
    private readonly string _queryTest = "Filter=Nome eq '{nome}' and contains(PalavrasChave, '{palavra-chave}') and Status eq '{status}'";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Feed _feedNovo;

    public PesquisarFeedTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _feedNovo = (FfkApi.Domain.Entities.Feed)entidades["FeedNovo"];
    }

    private string AjustaQuery(FfkApi.Domain.Entities.Feed feed)
    {
        return _queryTest.Replace("{nome}", feed.Nome.Replace("'", "''")).Replace("{palavra-chave}", SorteiaPalavraChave(feed.PalavrasChave!).Replace("'", "''")).Replace("{status}", feed.Status.ToString());
    }
    private string SorteiaPalavraChave(string palavrasChave)
    {
        var listaPalavras = palavrasChave.Split(',');
        return listaPalavras[new Random().Next(0, listaPalavras.Length)].Trim();
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = AjustaQuery(_feedNovo);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayFeeds = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayFeeds.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiroFeed = arrayFeeds.FirstOrDefault();

        var id = primeiroFeed.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(_feedNovo.Id.ToString()));

        var nome = primeiroFeed.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(_feedNovo.Nome));

        var descricao = primeiroFeed.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(_feedNovo.Descricao));

        var palavrasChave = primeiroFeed.GetProperty("palavrasChave").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(palavrasChave));
        Assert.That(palavrasChave, Is.EqualTo(_feedNovo.PalavrasChave));

        var status = primeiroFeed.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(_feedNovo.Status.ToString()));

        var anexos = primeiroFeed.GetProperty("anexos").EnumerateArray();
        Assert.That(anexos, Is.Not.Null);

        var visibilidadeUsuarios = primeiroFeed.GetProperty("visibilidadeUsuarios").EnumerateArray();
        Assert.That(visibilidadeUsuarios, Is.Not.Null);
        Assert.That(visibilidadeUsuarios.ToListString(), Is.EquivalentTo(_feedNovo.VisibilidadeUsuarios.Select(usuario => usuario.Email).ToList()));

        var visibilidadeEquipes = primeiroFeed.GetProperty("visibilidadeEquipes").EnumerateArray();
        Assert.That(visibilidadeEquipes, Is.Not.Null);
        Assert.That(visibilidadeEquipes.ToListString(), Is.EquivalentTo(_feedNovo.VisibilidadeEquipes.Select(equipe => equipe.Nome).ToList()));

        var expiraEm = primeiroFeed.GetProperty("expiraEm").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(expiraEm));
        Assert.That(expiraEm, Is.EqualTo(_feedNovo.ExpiraEm!.Value.ToString("dd/MM/yyyy")));

        var organizacao = primeiroFeed.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_feedNovo.Organizacao.Nome));

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var query = AjustaQuery(_feedNovo);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayFeeds = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayFeeds.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiroFeed = arrayFeeds.FirstOrDefault();

        var id = primeiroFeed.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(_feedNovo.Id.ToString()));

        var nome = primeiroFeed.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(_feedNovo.Nome));

        var descricao = primeiroFeed.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(_feedNovo.Descricao));

        var palavrasChave = primeiroFeed.GetProperty("palavrasChave").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(palavrasChave));
        Assert.That(palavrasChave, Is.EqualTo(_feedNovo.PalavrasChave));

        var status = primeiroFeed.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(_feedNovo.Status.ToString()));

        var anexos = primeiroFeed.GetProperty("anexos").EnumerateArray();
        Assert.That(anexos, Is.Not.Null);

        var visibilidadeUsuarios = primeiroFeed.GetProperty("visibilidadeUsuarios").EnumerateArray();
        Assert.That(visibilidadeUsuarios, Is.Not.Null);
        Assert.That(visibilidadeUsuarios.ToListString(), Is.EquivalentTo(_feedNovo.VisibilidadeUsuarios.Select(usuario => usuario.Email).ToList()));

        var visibilidadeEquipes = primeiroFeed.GetProperty("visibilidadeEquipes").EnumerateArray();
        Assert.That(visibilidadeEquipes, Is.Not.Null);
        Assert.That(visibilidadeEquipes.ToListString(), Is.EquivalentTo(_feedNovo.VisibilidadeEquipes.Select(equipe => equipe.Nome).ToList()));

        var expiraEm = primeiroFeed.GetProperty("expiraEm").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(expiraEm));
        Assert.That(expiraEm, Is.EqualTo(_feedNovo.ExpiraEm!.Value.ToString("dd/MM/yyyy")));

        var organizacao = primeiroFeed.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_feedNovo.Organizacao.Nome));

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Erro_Feed_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = "Filter=Nome eq ''";

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("FEED_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var query = _queryTest;

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, "tokenInvalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var query = _queryTest;

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_TOKEN");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_De_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(Guid.NewGuid());

        var query = _queryTest;

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Expirado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2Ns"
                + "YWltcy9zaWQiOiIwNzBhMjYzYy1hODViLTQyOWQtODM5Ny1iYTBjMTZlNjYyOTAiLCJuYmYiOjE3NDU4NDE1MjMsImV4cCI6MTc0NTg0MTU4M"
                + "iwiaWF0IjoxNzQ1ODQxNTIzfQ.zgcOTtirTevb3SgdvDerGUt25TAR079ps0vNIOQHZ4g";

        var query = _queryTest;

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var erros = dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();

        var mensagemEsperada = MessagesException.GetString("TOKEN_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
        Assert.That(dadosDaResposta.RootElement.GetProperty("tokenEstaExpirado").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = _queryTest;

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token, addAppToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_AUSENTE");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = _queryTest;

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
