using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Integracao.Test.Organizacao.Pesquisar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PesquisarOrganizacaoTest : FfkApiClassFixture
{
    private readonly string _baseUrlPesquisar = "organizacao/pesquisar";
    private readonly string _queryTest = "Filter=Nome eq '{nome}' or contains(Descricao, '{descricao}')&OrderBy=Nome desc&Top=2&Skip=0";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Organizacao _organizacaoFfkApi;
    private readonly FfkApi.Domain.Entities.Organizacao _organizacaoNova;

    public PesquisarOrganizacaoTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _organizacaoFfkApi = (FfkApi.Domain.Entities.Organizacao)entidades["OrganizacaoFfkApi"];
        _organizacaoNova = (FfkApi.Domain.Entities.Organizacao)entidades["OrganizacaoNova"];
    }

    private string AjustaQuery(FfkApi.Domain.Entities.Organizacao organizacao)
    {
        return _queryTest.Replace("{nome}", organizacao.Nome.Replace("'", "''")).Replace("{descricao}", SorteiaPalavraDaFrase(organizacao.Descricao).Replace("'", "''"));
    }

    private string SorteiaPalavraDaFrase(string frase)
    {
        var listaPalavras = frase.Split(' ');
        return listaPalavras[new Random().Next(0, listaPalavras.Length)];
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = AjustaQuery(_organizacaoFfkApi);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayOrganizacoes = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayOrganizacoes.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiroOrganizacao = arrayOrganizacoes.FirstOrDefault();
        Assert.That(!string.IsNullOrWhiteSpace(primeiroOrganizacao.GetProperty("id").GetString()));
        Assert.That(!string.IsNullOrWhiteSpace(primeiroOrganizacao.GetProperty("nome").GetString()));
        Assert.That(!string.IsNullOrWhiteSpace(primeiroOrganizacao.GetProperty("descricao").GetString()));

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Erro_Usuario_Nao_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var query = AjustaQuery(_organizacaoFfkApi);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SOMENTE_ADMINISTRADOR");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = "Filter=Nome eq ''";

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var query = AjustaQuery(_organizacaoFfkApi);

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

        var query = AjustaQuery(_organizacaoFfkApi);

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

        var query = AjustaQuery(_organizacaoFfkApi);

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

        var query = AjustaQuery(_organizacaoFfkApi);

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

        var query = AjustaQuery(_organizacaoFfkApi);

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

        var query = AjustaQuery(_organizacaoFfkApi);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
