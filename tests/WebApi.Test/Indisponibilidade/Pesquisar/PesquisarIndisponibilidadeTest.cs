using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using System.Text.Json;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Integracao.Test.Indisponibilidade.Pesquisar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PesquisarIndisponibilidadeTest : FfkApiClassFixture
{
    private readonly string _baseUrlPesquisar = "indisponibilidade/pesquisar";
    private readonly string _queryTest = "Filter=Usuario/Nome eq '{nome-usuario}' and contains(Descricao, '{descricao}')&OrderBy=Usuario/Nome desc&Top=2&Skip=0";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroIndisponibilidades;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Indisponibilidade _indisponibilidadeNova;
    private readonly FfkApi.Domain.Entities.Indisponibilidade _indisponibilidadeNovaOrganizacaoNova;

    public PesquisarIndisponibilidadeTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroIndisponibilidades = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroIndisponibilidades"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _indisponibilidadeNova = (FfkApi.Domain.Entities.Indisponibilidade)entidades["IndisponibilidadeNova"];
        _indisponibilidadeNovaOrganizacaoNova = (FfkApi.Domain.Entities.Indisponibilidade)entidades["IndisponibilidadeNovaOrganizacaoNova"];
    }

    private string AjustaQuery(FfkApi.Domain.Entities.Indisponibilidade indisponibilidade)
    {
        return _queryTest.Replace("{nome-usuario}", indisponibilidade.Usuario.Nome)
            .Replace("{descricao}", SorteiaPalavraDaFrase(indisponibilidade.Descricao));
    }

    private static string SorteiaPalavraDaFrase(string frase)
    {
        var listaPalavras = frase.Split(' ');
        return listaPalavras[new Random().Next(0, listaPalavras.Length)];
    }

    private void AssertJsonElementComIndisponibilidade(JsonElement jsonElement, FfkApi.Domain.Entities.Indisponibilidade indisponibilidade)
    {
        var id = jsonElement.GetProperty("id").GetString();
        Assert.That(id, Is.EqualTo(indisponibilidade.Id.ToString()));

        var descricao = jsonElement.GetProperty("descricao").GetString();
        Assert.That(descricao, Is.EqualTo(indisponibilidade.Descricao));

        var dataInicial = jsonElement.GetProperty("dataInicial").GetString();
        Assert.That(dataInicial, Is.EqualTo(indisponibilidade.DataInicial.ToString("dd/MM/yyyy")));

        var dataFinal = jsonElement.GetProperty("dataFinal").GetString();
        Assert.That(dataFinal, Is.EqualTo(indisponibilidade.DataFinal.ToString("dd/MM/yyyy")));

        var idUsuario = jsonElement.GetProperty("usuario").GetProperty("id").GetString();
        Assert.That(idUsuario, Is.EqualTo(indisponibilidade.Usuario.Id.ToString()));

        var nomeUsuario = jsonElement.GetProperty("usuario").GetProperty("nome").GetString();
        Assert.That(nomeUsuario, Is.EqualTo(indisponibilidade.Usuario.Nome));

        var emailUsuario = jsonElement.GetProperty("usuario").GetProperty("email").GetString();
        Assert.That(emailUsuario, Is.EqualTo(indisponibilidade.Usuario.Email));

        var organizacaoUsuario = jsonElement.GetProperty("usuario").GetProperty("organizacao").GetString();
        Assert.That(organizacaoUsuario, Is.EqualTo(indisponibilidade.Usuario.Organizacao.Nome));
    }

    [Test]
    public async Task Sucesso_Administrador_Pesquisando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = AjustaQuery(_indisponibilidadeNovaOrganizacaoNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayIndisponibilidades = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayIndisponibilidades.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiraIndisponibilidade = arrayIndisponibilidades.FirstOrDefault();

        AssertJsonElementComIndisponibilidade(primeiraIndisponibilidade, _indisponibilidadeNovaOrganizacaoNova);

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Pesquisando_Para_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroIndisponibilidades.Id);

        var query = AjustaQuery(_indisponibilidadeNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayIndisponibilidades = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayIndisponibilidades.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiraIndisponibilidade = arrayIndisponibilidades.FirstOrDefault();

        AssertJsonElementComIndisponibilidade(primeiraIndisponibilidade, _indisponibilidadeNova);

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Pesquisando_Para_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var query = AjustaQuery(_indisponibilidadeNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arrayIndisponibilidades = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arrayIndisponibilidades.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiraIndisponibilidade = arrayIndisponibilidades.FirstOrDefault();

        AssertJsonElementComIndisponibilidade(primeiraIndisponibilidade, _indisponibilidadeNova);

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Pesquisando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroIndisponibilidades.Id);

        var query = AjustaQuery(_indisponibilidadeNovaOrganizacaoNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("INDISPONIBILIDADE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Perfil_Nem_Permissao_Pesquisando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var query = AjustaQuery(_indisponibilidadeNovaOrganizacaoNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("INDISPONIBILIDADE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Indisponibilidade_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = "Filter=Usuario/Nome eq ''";

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("INDISPONIBILIDADE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var query = AjustaQuery(_indisponibilidadeNova);

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

        var query = AjustaQuery(_indisponibilidadeNova);

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

        var query = AjustaQuery(_indisponibilidadeNova);

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

        var query = AjustaQuery(_indisponibilidadeNova);

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

        var query = AjustaQuery(_indisponibilidadeNova);

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

        var query = AjustaQuery(_indisponibilidadeNova);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
