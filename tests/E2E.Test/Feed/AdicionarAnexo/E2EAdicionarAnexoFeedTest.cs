using FfkApi.Communication.Requests;
using FfkApi.Domain.Configurations;
using FfkApi.Exceptions;
using System.Net;
using System.Text.Json;
using TestUtil.Entities;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Feed.AdicionarAnexo;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EAdicionarAnexoFeedTest : E2EClassFixture
{
    private readonly string _baseUrlAdicionarAnexoFeed = "feed/anexo";

    private const long _tamanhoMaximoArquivo = 1024;

    [SetUp]
    public void SetUp()
    {
        ConfiguracaoArquivoAnexo.Inicializar(_tamanhoMaximoArquivo);
    }

    private static void AssertDadosDaRespostaComRequest(JsonDocument dadosDaResposta, RequestAdicionarAnexoFeed request)
    {
        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var nomeArquivo = dadosDaResposta.RootElement.GetProperty("nomeArquivo").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nomeArquivo));
        Assert.That(nomeArquivo, Is.EqualTo(request.Arquivo!.FileName));

        var tamanhoBytes = dadosDaResposta.RootElement.GetProperty("tamanhoBytes").GetInt64();
        Assert.That(tamanhoBytes, Is.Not.Null);
        Assert.That(tamanhoBytes, Is.EqualTo(request.Arquivo.Length));
    }

    [Test]
    public async Task Sucesso_Administrador_Arquivo_Pequeno_E_Feed_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var feedNovo = await CadastroHelper.CadastrarNovoFeed(organizacao: organizacaoNova);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feedNovo.Id.ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Sucesso_Administrador_Arquivo_Tamanho_Maximo_E_Feed_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = _tamanhoMaximoArquivo;

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var feedNovo = await CadastroHelper.CadastrarNovoFeed(organizacao: organizacaoNova);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feedNovo.Id.ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_E_Feed_Da_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroFeeds = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Feeds"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroFeeds.Id);

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var feedNovo = await CadastroHelper.CadastrarNovoFeed(organizacao: usuarioPermissaoCadastroFeeds.Organizacao);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feedNovo.Id.ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_E_Feed_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroFeeds = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Feeds"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroFeeds.Id);

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var feedNovo = await CadastroHelper.CadastrarNovoFeed(organizacao: organizacaoNova);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feedNovo.Id.ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("FEED_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Arquivo_Muito_Grande()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = _tamanhoMaximoArquivo + 1;

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feedNovo.Id.ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ARQUIVO_MUITO_GRANDE").Replace("{tamanho-maximo}", ConfiguracaoArquivoAnexo.TamanhoMaximoBytesTexto);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feedNovo.Id.ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Feeds");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, Guid.NewGuid().ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_TOKEN");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, Guid.NewGuid().ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, cancellationToken: cancellationToken, request: request, token: "TokenInvalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_De_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(Guid.NewGuid());

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, Guid.NewGuid().ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, cancellationToken: cancellationToken, request: request, token: token);

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

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, Guid.NewGuid().ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, cancellationToken: cancellationToken, request: request, token: token);

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

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, Guid.NewGuid().ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, Guid.NewGuid().ToString());

        var response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
