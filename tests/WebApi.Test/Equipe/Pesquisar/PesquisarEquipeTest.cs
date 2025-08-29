using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using System.Text.Json;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Integracao.Test.Equipe.Pesquisar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PesquisarEquipeTest : FfkApiClassFixture
{
    private readonly string _baseUrlPesquisar = "equipe/pesquisar";
    private readonly string _queryTest = "Filter=Nome eq '{nome}' and contains(Descricao, '{descricao}')&OrderBy=Nome&Top=2&Skip=0";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroEquipes;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Equipe _equipeNova;
    private readonly FfkApi.Domain.Entities.Organizacao _organizacaoNova;

    public PesquisarEquipeTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroEquipes = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroEquipes"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _equipeNova = (FfkApi.Domain.Entities.Equipe)entidades["EquipeNova"];
        _organizacaoNova = (FfkApi.Domain.Entities.Organizacao)entidades["OrganizacaoNova"];
    }

    private string AjustaQuery(FfkApi.Domain.Entities.Equipe equipe)
    {
        return _queryTest.Replace("{nome}", equipe.Nome.Replace("'", "''")).Replace("{descricao}", SorteiaPalavraDaFrase(equipe.Descricao).Replace("'", "''"));
    }

    private string SorteiaPalavraDaFrase(string frase)
    {
        var listaPalavras = frase.Split(' ');
        return listaPalavras[new Random().Next(0, listaPalavras.Length)];
    }

    private static void AssertJsonElementComEquipe(JsonElement jsonElement, FfkApi.Domain.Entities.Equipe equipe)
    {
        var id = jsonElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(equipe.Id.ToString()));

        var nome = jsonElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(equipe.Nome));

        var descricao = jsonElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(equipe.Descricao));

        var status = jsonElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(equipe.Status.ToString()));

        var organizacao = jsonElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(equipe.Organizacao.Nome));

        var equipeNovaMembrosByEmail = equipe.Membros!
            .ToDictionary(m => m.Usuario.Email);
        var responseMembros = jsonElement.GetProperty("membros").EnumerateArray();
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
    public async Task Sucesso_Administrador_Pesquisando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuarioLogin: _usuarioAdministrador,
            usuariosEquipe: [],
            organizacao: _organizacaoNova);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = AjustaQuery(equipe);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayEquipes = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayEquipes.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiraEquipe = arrayEquipes.FirstOrDefault();
        AssertJsonElementComEquipe(primeiraEquipe, equipe);

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Pesquisando_Equipe_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroEquipes.Id);

        var query = AjustaQuery(_equipeNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayEquipes = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayEquipes.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiraEquipe = arrayEquipes.FirstOrDefault();
        AssertJsonElementComEquipe(primeiraEquipe, _equipeNova);

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Pesquisando_Equipe_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var query = AjustaQuery(_equipeNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayEquipes = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayEquipes.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiraEquipe = arrayEquipes.FirstOrDefault();
        AssertJsonElementComEquipe(primeiraEquipe, _equipeNova);

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Pesquisando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuarioLogin: _usuarioAdministrador,
            usuariosEquipe: [],
            organizacao: _organizacaoNova);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroEquipes.Id);

        var query = AjustaQuery(equipe);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EQUIPE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Pesquisando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuarioLogin: _usuarioAdministrador,
            usuariosEquipe: [],
            organizacao: _organizacaoNova);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var query = AjustaQuery(equipe);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

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

        var query = "Filter=Nome eq ''";

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

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

        var query = AjustaQuery(_equipeNova);

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

        var query = AjustaQuery(_equipeNova);

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

        var query = AjustaQuery(_equipeNova);

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

        var query = AjustaQuery(_equipeNova);

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

        var query = AjustaQuery(_equipeNova);

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

        var query = AjustaQuery(_equipeNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
