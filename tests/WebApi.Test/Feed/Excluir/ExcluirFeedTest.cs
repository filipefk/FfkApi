using FfkApi.Domain.Configurations;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.Entities;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Integracao.Test.Feed.Excluir;

[TestFixture]
public class ExcluirFeedTest : FfkApiClassFixture
{
    private readonly string _baseUrlFeed = "feed";
    private readonly string _baseUrlAdicionarAnexoFeed = "feed/anexo";
    private readonly string _baseUrlAnexo = "anexo";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroFeeds;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Feed _feedNovo;
    private readonly FfkApi.Domain.Entities.Organizacao _organizacaoFfkApi;

    private const long _tamanhoMaximoArquivo = 1024;

    [SetUp]
    public void SetUp()
    {
        ConfiguracaoArquivoAnexo.Inicializar(_tamanhoMaximoArquivo);
    }

    public ExcluirFeedTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroFeeds = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroFeeds"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _feedNovo = (FfkApi.Domain.Entities.Feed)entidades["FeedNovo"];
        _organizacaoFfkApi = (FfkApi.Domain.Entities.Organizacao)entidades["OrganizacaoFfkApi"];
    }

    private async Task<FfkApi.Domain.Entities.Feed> CadastrarNovoFeed(int quantAnexos = 0)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var palavrasChave = dadosDaResposta.RootElement.GetProperty("palavrasChave").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(palavrasChave));
        Assert.That(palavrasChave, Is.EqualTo(request.PalavrasChave));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(request.Status));

        var visibilidadeUsuarios = dadosDaResposta.RootElement.GetProperty("visibilidadeUsuarios").EnumerateArray();
        Assert.That(visibilidadeUsuarios, Is.Not.Null);
        Assert.That(visibilidadeUsuarios.ToListString(), Is.EquivalentTo(request.VisibilidadeUsuarios!));

        var visibilidadeEquipes = dadosDaResposta.RootElement.GetProperty("visibilidadeEquipes").EnumerateArray();
        Assert.That(visibilidadeEquipes, Is.Not.Null);
        Assert.That(visibilidadeEquipes.ToListString(), Is.EquivalentTo(request.VisibilidadeEquipes!));

        var expiraEm = dadosDaResposta.RootElement.GetProperty("expiraEm").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(expiraEm));
        Assert.That(expiraEm, Is.EqualTo(request.ExpiraEm));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(request.Organizacao));

        var novoFeed = new FfkApi.Domain.Entities.Feed
        {
            Id = Guid.Parse(id!),
            Nome = nome!,
            Descricao = descricao!,
            PalavrasChave = palavrasChave!,
            Status = EnumUtil.ConverterTextoParaEnum<StatusFeed>(status!),
            VisibilidadeUsuarios = [],
            VisibilidadeEquipes = [],
            ExpiraEm = DateOnly.ParseExact(expiraEm!, "dd/MM/yyyy"),
            IdOrganizacao = _organizacaoFfkApi.Id,
            Organizacao = _organizacaoFfkApi
        };

        for (int i = 0; i < quantAnexos; i++)
        {
            var anexo = AnexoBuilder.Build();
            anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

            var requestAnexoFeed = RequestAdicionarAnexoFeedBuilder.Build(anexo, novoFeed.Id.ToString());

            response = await HttpHelper.DoPostCadastrarAnexo(url: _baseUrlAdicionarAnexoFeed, request: requestAnexoFeed, token: token, cancellationToken: cancellationToken);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

            id = dadosDaResposta.RootElement.GetProperty("id").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(id));

            nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(nome));
            Assert.That(nome, Is.EqualTo(requestAnexoFeed.Nome));

            descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(descricao));
            Assert.That(descricao, Is.EqualTo(requestAnexoFeed.Descricao));

            var nomeArquivo = dadosDaResposta.RootElement.GetProperty("nomeArquivo").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(nomeArquivo));
            Assert.That(nomeArquivo, Is.EqualTo(requestAnexoFeed.Arquivo!.FileName));

            var tamanhoBytes = dadosDaResposta.RootElement.GetProperty("tamanhoBytes").GetInt64();
            Assert.That(tamanhoBytes, Is.Not.Null);
            Assert.That(tamanhoBytes, Is.EqualTo(requestAnexoFeed.Arquivo.Length));

            anexo.Id = Guid.Parse(id!);

            novoFeed.Anexos.Add(anexo);
        }

        return novoFeed;
    }

    private async Task<bool> ExisteAnexo(string idAnexo)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlAnexo}/{idAnexo}", cancellationToken, token);

        return response.StatusCode == HttpStatusCode.OK;
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var feed = await CadastrarNovoFeed(1);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlFeed}/{feed.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var listaAnexos = dadosDaResposta.RootElement.GetProperty("anexos").EnumerateArray();
        Assert.That(listaAnexos.Count, Is.EqualTo(1));
        var idAnexo = listaAnexos.FirstOrDefault().GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(idAnexo));
        Assert.That(await ExisteAnexo(idAnexo!));

        response = await HttpHelper.DoDelete(url: $"{_baseUrlFeed}/{feed.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        response = await HttpHelper.DoGet(url: $"{_baseUrlFeed}/{feed.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        Assert.That(!await ExisteAnexo(idAnexo!));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroFeeds.Id);

        var feed = await CadastrarNovoFeed(1);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlFeed}/{feed.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var listaAnexos = dadosDaResposta.RootElement.GetProperty("anexos").EnumerateArray();
        Assert.That(listaAnexos.Count, Is.EqualTo(1));
        var idAnexo = listaAnexos.FirstOrDefault().GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(idAnexo));
        Assert.That(await ExisteAnexo(idAnexo!));

        response = await HttpHelper.DoDelete(url: $"{_baseUrlFeed}/{feed.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        response = await HttpHelper.DoGet(url: $"{_baseUrlFeed}/{feed.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        Assert.That(!await ExisteAnexo(idAnexo!));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var feed = await CadastrarNovoFeed(1);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlFeed}/{feed.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var listaAnexos = dadosDaResposta.RootElement.GetProperty("anexos").EnumerateArray();
        Assert.That(listaAnexos.Count, Is.EqualTo(1));
        var idAnexo = listaAnexos.FirstOrDefault().GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(idAnexo));
        Assert.That(await ExisteAnexo(idAnexo!));

        token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        response = await HttpHelper.DoDelete(url: $"{_baseUrlFeed}/{feed.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Feeds");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Feed_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlFeed}/{Guid.NewGuid()}", token: token, cancellationToken: cancellationToken);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlFeed}/{_feedNovo.Id}", cancellationToken, "tokenInvalid");

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlFeed}/{id}", cancellationToken: cancellationToken, token: token);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlFeed}/{_feedNovo.Id}", cancellationToken);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlFeed}/{_feedNovo.Id}", cancellationToken, token);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlFeed}/{_feedNovo.Id}", cancellationToken, token);

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlFeed}/{_feedNovo.Id}", token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlFeed}/{_feedNovo.Id}", token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
