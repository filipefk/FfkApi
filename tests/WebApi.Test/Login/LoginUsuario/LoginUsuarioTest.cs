using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;

namespace Integracao.Test.Login.LoginUsuario;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LoginUsuarioTest : FfkApiClassFixture
{
    private readonly string _baseUrl = "login";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;

    public LoginUsuarioTest()
    {
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)_factory.EntidadesCriadas()["UsuarioSemPerfilNemPermissao"];
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestLoginUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPost(_baseUrl, cancellationToken, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var name = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(name));
        Assert.That(name, Is.EqualTo(_usuarioSemPerfilNemPermissao.Nome));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString()));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString()));

    }

    [Test]
    public async Task Erro_Login_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestLoginUsuarioBuilder.Build();

        var response = await HttpHelper.DoPost(_baseUrl, cancellationToken, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAIL_OU_SENHA_INVALIDOS");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestLoginUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);

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

        var request = RequestLoginUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPost(_baseUrl, cancellationToken, request, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
