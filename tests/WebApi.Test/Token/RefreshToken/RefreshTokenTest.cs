using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;

namespace Integracao.Test.Token.RefreshToken;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class RefreshTokenTest : FfkApiClassFixture
{
    private readonly string _baseUrlLogin = "login";
    private readonly string _baseUrlRefreshToken = "token/refresh-token";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroUsuarios;

    public RefreshTokenTest()
    {
        _usuarioPermissaoCadastroUsuarios = (FfkApi.Domain.Entities.Usuario)_factory.EntidadesCriadas()["UsuarioPermissaoCadastroUsuarios"];
    }

    private async Task<string> PegarRefreshTokenDoLogin()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var requestLogin = RequestLoginUsuarioBuilder.Build(_usuarioPermissaoCadastroUsuarios);

        var responseLogin = await HttpHelper.DoPost(_baseUrlLogin, cancellationToken, requestLogin);

        Assert.That(responseLogin.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseLoginData = await HttpResponseUtil.PegarDadosDaResposta(responseLogin);

        return responseLoginData.RootElement
            .GetProperty("tokens").GetProperty("refreshToken").GetString()!;
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var refreshTokenLogin = await PegarRefreshTokenDoLogin();

        var request = new RequestNovoTokenUsuario
        {
            RefreshToken = refreshTokenLogin!
        };

        var response = await HttpHelper.DoPost(_baseUrlRefreshToken, cancellationToken, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("accessToken").GetString()));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("refreshToken").GetString()));
    }

    [Test]
    public async Task Erro_Refresh_Token_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var requestNewToken = new RequestNovoTokenUsuario
        {
            RefreshToken = "TOKEN_INVALIDO"
        };

        var responseRefreshToken = await HttpHelper.DoPost(_baseUrlRefreshToken, cancellationToken, requestNewToken);

        Assert.That(responseRefreshToken.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var errors = await HttpResponseUtil.PegarMensagensDeErro(responseRefreshToken);

        var mensagemEsperada = MessagesException.GetString("SESSAO_EXPIRADA");

        Assert.That(errors.Count(), Is.EqualTo(1));
        Assert.That(errors.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }


    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var refreshTokenLogin = await PegarRefreshTokenDoLogin();

        var request = new RequestNovoTokenUsuario
        {
            RefreshToken = refreshTokenLogin!
        };

        var response = await HttpHelper.DoPost(_baseUrlRefreshToken, cancellationToken, request, addAppToken: false);

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

        var refreshTokenLogin = await PegarRefreshTokenDoLogin();

        var request = new RequestNovoTokenUsuario
        {
            RefreshToken = refreshTokenLogin!
        };

        var response = await HttpHelper.DoPost(_baseUrlRefreshToken, cancellationToken, request, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
