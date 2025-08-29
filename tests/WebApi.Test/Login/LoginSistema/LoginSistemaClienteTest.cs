using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;

namespace Integracao.Test.Login.LoginSistema;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LoginSistemaClienteTest : FfkApiClassFixture
{
    private readonly string _baseUrl = "login/sistema";
    private readonly FfkApi.Domain.Entities.SistemaCliente _sistemaClienteNovo;

    public LoginSistemaClienteTest()
    {
        _sistemaClienteNovo = (FfkApi.Domain.Entities.SistemaCliente)_factory.EntidadesCriadas()["SistemaClienteNovo"];
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestLoginSistemaClienteBuilder.Build(_sistemaClienteNovo);

        var response = await HttpHelper.DoPost(_baseUrl, cancellationToken, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var name = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(name));
        Assert.That(name, Is.EqualTo(_sistemaClienteNovo.Nome));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("accessToken").GetString()));
    }

    [Test]
    public async Task Erro_Login_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestLoginSistemaClienteBuilder.Build();

        var response = await HttpHelper.DoPost(_baseUrl, cancellationToken, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("APP_ID_OU_SENHA_INVALIDOS");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestLoginSistemaClienteBuilder.Build(_sistemaClienteNovo);

        var response = await HttpHelper.DoPost(_baseUrl, cancellationToken, request, addAppToken: false);

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

        var request = RequestLoginSistemaClienteBuilder.Build(_sistemaClienteNovo);

        var response = await HttpHelper.DoPost(_baseUrl, cancellationToken, request, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
