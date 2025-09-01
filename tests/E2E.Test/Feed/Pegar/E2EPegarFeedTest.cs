using FfkApi.Exceptions;
using System.Net;
using System.Text.Json;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Aceitacao.Test.Feed.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EPegarFeedTest : E2EClassFixture
{
    private readonly string _baseUrlFeed = "feed";

    private static void AssertDadosDaRespostaComFeed(JsonDocument dadosDaResposta, FfkApi.Domain.Entities.Feed feed)
    {
        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(feed.Id.ToString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(feed.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(feed.Descricao));

        var palavrasChave = dadosDaResposta.RootElement.GetProperty("palavrasChave").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(palavrasChave));
        Assert.That(palavrasChave, Is.EqualTo(feed.PalavrasChave));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(feed.Status.ToString()));

        var anexos = dadosDaResposta.RootElement.GetProperty("anexos").EnumerateArray();
        Assert.That(anexos, Is.Not.Null);

        var visibilidadeUsuarios = dadosDaResposta.RootElement.GetProperty("visibilidadeUsuarios").EnumerateArray();
        Assert.That(visibilidadeUsuarios, Is.Not.Null);
        Assert.That(visibilidadeUsuarios.ToListString(), Is.EquivalentTo(feed.VisibilidadeUsuarios.Select(usuario => usuario.Email).ToList()));

        var visibilidadeEquipes = dadosDaResposta.RootElement.GetProperty("visibilidadeEquipes").EnumerateArray();
        Assert.That(visibilidadeEquipes, Is.Not.Null);
        Assert.That(visibilidadeEquipes.ToListString(), Is.EquivalentTo(feed.VisibilidadeEquipes.Select(equipe => equipe.Nome).ToList()));

        var expiraEm = dadosDaResposta.RootElement.GetProperty("expiraEm").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(expiraEm));
        Assert.That(expiraEm, Is.EqualTo(feed.ExpiraEm!.Value.ToString("dd/MM/yyyy")));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(feed.Organizacao.Nome));
    }

    [Test]
    public async Task Sucesso_Administrador_Pegando_Feed_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var feedNovo = await CadastroHelper.CadastrarNovoFeed(organizacao: organizacaoNova);

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{feedNovo.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComFeed(dadosDaResposta, feedNovo);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Pegando_Feed_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var feedNovo = await CadastroHelper.CadastrarNovoFeed(organizacao: _usuarioSemPerfilNemPermissao.Organizacao);

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{feedNovo.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComFeed(dadosDaResposta, feedNovo);
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Pegando_Feed_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroFeeds = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Feeds"], ativar: true);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var feedNovo = await CadastroHelper.CadastrarNovoFeed(
            organizacao: organizacaoNova);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{feedNovo.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComFeed(dadosDaResposta, feedNovo);

        token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroFeeds.Id);

        response = await HttpHelper.DoGet($"{_baseUrlFeed}/{feedNovo.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("FEED_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Feed_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{Guid.NewGuid()}", cancellationToken, "tokenInvalid");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlFeed}/{id}", cancellationToken: cancellationToken, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ID_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{Guid.NewGuid()}", cancellationToken);

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

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{Guid.NewGuid()}", cancellationToken, token, addAppToken: false);

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

        var response = await HttpHelper.DoGet($"{_baseUrlFeed}/{Guid.NewGuid()}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
