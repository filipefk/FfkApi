using FfkApi.Exceptions;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Aceitacao.Test.SistemaCliente.Pesquisar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EPesquisarSistemaClienteTest : E2EClassFixture
{
    private readonly string _baseUrlPesquisar = "sistemaCliente/pesquisar";
    private readonly string _queryTest = "Filter=Nome eq '{nome}' and contains(Descricao, '{descricao}')&OrderBy=Nome desc&Top=2&Skip=0";

    private string AjustaQuery(FfkApi.Domain.Entities.SistemaCliente sistemaCliente)
    {
        return _queryTest.Replace("{nome}", sistemaCliente.Nome.Replace("'", "''")).Replace("{descricao}", SorteiaPalavraDaFrase(sistemaCliente.Descricao).Replace("'", "''"));
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

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var query = AjustaQuery(sistemaCliente);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var arraySistemasCliente = dadosDaResposta.RootElement.GetProperty("resultados").EnumerateArray();
        Assert.That(arraySistemasCliente.Count(), Is.GreaterThanOrEqualTo(1));

        var primeiroSistemaCliente = arraySistemasCliente.FirstOrDefault();

        var id = primeiroSistemaCliente.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(sistemaCliente.Id.ToString()));

        var appId = primeiroSistemaCliente.GetProperty("appId").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(appId));
        Assert.That(appId, Is.EqualTo(sistemaCliente.AppId.ToString()));

        var nome = primeiroSistemaCliente.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(sistemaCliente.Nome));

        var descricao = primeiroSistemaCliente.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(sistemaCliente.Descricao));

        var status = primeiroSistemaCliente.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(sistemaCliente.Status.ToString()));

        Assert.That(dadosDaResposta.RootElement.GetProperty("paginaAtual").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("totalDePaginas").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("tamanhoDaPagina").GetUInt16() > 0);
        Assert.That(dadosDaResposta.RootElement.GetProperty("quantidadeTotal").GetUInt16() > 0);
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var query = AjustaQuery(sistemaCliente);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SOMENTE_ADMINISTRADOR");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_SistemaCliente_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var query = "Filter=Nome eq ''";

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SISTEMACLIENTE_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var query = AjustaQuery(sistemaCliente);

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

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var query = AjustaQuery(sistemaCliente);

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

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var query = AjustaQuery(sistemaCliente);

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

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var query = AjustaQuery(sistemaCliente);

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

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var query = AjustaQuery(sistemaCliente);

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

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var query = AjustaQuery(sistemaCliente);

        var response = await HttpHelper.DoGet($"{_baseUrlPesquisar}?{query}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
