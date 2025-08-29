using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using System.Net;
using System.Text.Json;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Equipe.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2ECadastrarEquipeTest : E2EClassFixture
{
    private readonly string _baseUrlEquipe = "equipe";

    private static void AssertDadosDaRespostaComRequest(JsonDocument dadosDaResposta, RequestCadastrarEquipe request)
    {
        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(request.Organizacao));

        var requestMembrosByEmail = request.Membros!
            .Where(m => m.Email != null)
            .ToDictionary(m => m.Email!);
        var membros = dadosDaResposta.RootElement.GetProperty("membros").EnumerateArray();
        Assert.That(membros.Count, Is.EqualTo(request.Membros!.Count));
        var responseMembrosByEmail = membros
            .ToDictionary(
                c => c.GetProperty("email").GetString()!,
                c => c
            );
        Assert.That(responseMembrosByEmail.Keys, Is.EquivalentTo(requestMembrosByEmail.Keys));

        foreach (var email in responseMembrosByEmail.Keys)
        {
            var responseMembro = responseMembrosByEmail[email];
            var requestMembro = requestMembrosByEmail[email];

            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("id").GetString()));
            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("idUsuario").GetString()));
            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("nome").GetString()));
            Assert.That(responseMembro.GetProperty("lider").GetBoolean(), Is.EqualTo(requestMembro.Lider!.Value), $"Lider diferente para o email {email}");
        }
    }

    [Test]
    public async Task Sucesso_Administrador_Cadastrando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = organizacaoNova.Nome;
        request.Membros = [];

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Sem_Informar_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroEquipes = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Equipes"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroEquipes.Id);

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = null;
        request.Membros = [RequestMembroEquipeBuilder.Build(_usuarioAdministrador, true), RequestMembroEquipeBuilder.Build(_usuarioSemPerfilNemPermissao, false)];

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);
        request.Organizacao = usuarioPermissaoCadastroEquipes.Organizacao.Nome;
        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Informando_A_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroEquipes = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Equipes"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroEquipes.Id);

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = usuarioPermissaoCadastroEquipes.Organizacao.Nome;
        request.Membros = [RequestMembroEquipeBuilder.Build(_usuarioAdministrador, true), RequestMembroEquipeBuilder.Build(_usuarioSemPerfilNemPermissao, false)];

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);
        request.Organizacao = usuarioPermissaoCadastroEquipes.Organizacao.Nome;
        AssertDadosDaRespostaComRequest(dadosDaResposta, request);
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Informando_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroEquipes = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Equipes"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroEquipes.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = organizacaoNova.Nome;
        request.Membros = [];

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestCadastrarEquipeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Equipes");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = "Organização Inexistente";
        request.Membros = [];

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Emails_Usuarios_Nao_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = null;
        var membroEquipe = RequestMembroEquipeBuilder.Build();
        request.Membros = [membroEquipe];

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAILS_DE_USUARIOS_NAO_ENCONTRADOS").Replace("{lista}", membroEquipe.Email);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nome_De_Equipe_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = null;
        request.Membros = [RequestMembroEquipeBuilder.Build(_usuarioAdministrador, true), RequestMembroEquipeBuilder.Build(_usuarioSemPerfilNemPermissao, false)];

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;

        AssertDadosDaRespostaComRequest(dadosDaResposta, request);

        var nomeAnterior = request.Nome;

        request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = null;
        request.Nome = nomeAnterior;
        request.Membros = [RequestMembroEquipeBuilder.Build(_usuarioAdministrador, true), RequestMembroEquipeBuilder.Build(_usuarioSemPerfilNemPermissao, false)];

        response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("NOME_DE_EQUIPE_JA_EXISTE_NA_ORGANIZACAO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("      ")]
    public async Task Erro_Descricao_Vazia(string? descricao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = null;
        request.Descricao = descricao;
        request.Membros = [RequestMembroEquipeBuilder.Build(_usuarioAdministrador, true), RequestMembroEquipeBuilder.Build(_usuarioSemPerfilNemPermissao, false)];

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DESCRICAO_VAZIA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarEquipeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, cancellationToken: cancellationToken, request: request);

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

        var request = RequestCadastrarEquipeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, cancellationToken: cancellationToken, request: request, token: "TokenInvalido");

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

        var request = RequestCadastrarEquipeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestCadastrarEquipeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestCadastrarEquipeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var request = RequestCadastrarEquipeBuilder.Build();

        var response = await HttpHelper.DoPost(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
