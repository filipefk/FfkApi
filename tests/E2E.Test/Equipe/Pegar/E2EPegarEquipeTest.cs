using FfkApi.Exceptions;
using System.Net;
using System.Text.Json;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Aceitacao.Test.Equipe.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EPegarEquipeTest : E2EClassFixture
{
    private readonly string _baseUrlEquipe = "equipe";

    private static void AssertDadosDaRespostaComEquipe(JsonDocument dadosDaResposta, FfkApi.Domain.Entities.Equipe equipe)
    {
        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(equipe.Id.ToString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(equipe.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(equipe.Descricao));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(equipe.Status.ToString()));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(equipe.Organizacao.Nome));

        var equipeNovaMembrosByEmail = equipe.Membros!
            .ToDictionary(m => m.Usuario.Email);
        var responseMembros = dadosDaResposta.RootElement.GetProperty("membros").EnumerateArray();
        Assert.That(responseMembros.Count, Is.EqualTo(equipe.Membros!.Count));
        var responseMembrosByEmail = responseMembros
            .ToDictionary(
                c => c.GetProperty("email").GetString()!,
                c => c
            );
        Assert.That(responseMembrosByEmail.Keys, Is.EquivalentTo(equipeNovaMembrosByEmail.Keys));

        foreach (var email in responseMembrosByEmail.Keys)
        {
            var responseMembro = responseMembrosByEmail[email];
            var equipeNovaMembro = equipeNovaMembrosByEmail[email];

            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("id").GetString()));
            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("idUsuario").GetString()));
            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.GetProperty("nome").GetString()));
            Assert.That(responseMembro.GetProperty("lider").GetBoolean(), Is.EqualTo(equipeNovaMembro.Lider), $"Lider diferente para o email {email}");
        }
    }

    [Test]
    public async Task Sucesso_Administrador_Pegando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();


        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [],
            organizacao: organizacaoNova);

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{equipe.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComEquipe(dadosDaResposta, equipe);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Pegando_Equipe_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroEquipes = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Equipes"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroEquipes.Id);



        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao],
            organizacao: usuarioPermissaoCadastroEquipes.Organizacao);

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{equipe.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComEquipe(dadosDaResposta, equipe);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Pegando_Equipe_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioSemPerfilNemPermissao = await CadastroHelper.CadastrarNovoUsuario(ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioSemPerfilNemPermissao.Id);



        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao],
            organizacao: usuarioSemPerfilNemPermissao.Organizacao);

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{equipe.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComEquipe(dadosDaResposta, equipe);
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Pegando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroEquipes = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Equipes"], ativar: true);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();


        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [],
            organizacao: organizacaoNova);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{equipe.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroEquipes.Id);

        response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{equipe.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EQUIPE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Equipe_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{Guid.NewGuid()}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EQUIPE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{Guid.NewGuid()}", cancellationToken, "tokenInvalid");

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

        var response = await HttpHelper.DoGet(url: $"{_baseUrlEquipe}/{id}", cancellationToken: cancellationToken, token: token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{Guid.NewGuid()}", cancellationToken);

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

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{Guid.NewGuid()}", cancellationToken, token, addAppToken: false);

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

        var response = await HttpHelper.DoGet($"{_baseUrlEquipe}/{Guid.NewGuid()}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
