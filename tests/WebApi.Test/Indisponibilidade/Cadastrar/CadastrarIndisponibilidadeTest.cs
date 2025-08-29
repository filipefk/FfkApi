using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using System.Text.Json;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Integracao.Test.Indisponibilidade.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarIndisponibilidadeTest : FfkApiClassFixture
{
    private readonly string _baseUrlIndisponibilidade = "indisponibilidade";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroIndisponibilidades;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova;

    public CadastrarIndisponibilidadeTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroIndisponibilidades = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroIndisponibilidades"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova"];
    }

    private static void AssertDadosDaRespostaComRequest(JsonDocument dadosDaResposta, RequestCadastrarIndisponibilidade request)
    {
        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var dataInicial = dadosDaResposta.RootElement.GetProperty("dataInicial").GetString();
        Assert.That(dataInicial, Is.EqualTo(request.DataInicial));

        var dataFinal = dadosDaResposta.RootElement.GetProperty("dataFinal").GetString();
        Assert.That(dataFinal, Is.EqualTo(request.DataFinal));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("id").GetString()));
        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("nome").GetString()));

        var email = dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("email").GetString();
        Assert.That(email, Is.EqualTo(request.Usuario));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("organizacao").GetString()));
    }

    [Test]
    public async Task Sucesso_Administrador_Cadastrando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova.Email;
        request.DataInicial = DateTime.Now.ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Cadastrando_Para_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroIndisponibilidades.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Cadastrando_Pra_Si_Mesmo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Cadastrando_Para_Outro_Usuario()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioPermissaoCadastroIndisponibilidades.Email;
        request.DataInicial = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Indisponibilidades");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Cadastrando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroIndisponibilidades.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova.Email;

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Existe_Indisponibilidade_Para_Usuario_No_Periodo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(4).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(4).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);

        request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(4).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(4).ToString("dd/MM/yyyy");

        response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("JA_EXISTE_INDISPONIBILIDADE_NO_PERIODO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase("Inválida")]
    [TestCase("31/02/2025")]
    [TestCase("01/15/2025")]
    public async Task Erro_Data_Inicial_Invalida(string? dataInicial)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = dataInicial;
        request.DataFinal = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DATA_INICIAL_INVALIDA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, cancellationToken: cancellationToken, request: request);

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

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, cancellationToken: cancellationToken, request: request, token: "TokenInvalido");

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

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
