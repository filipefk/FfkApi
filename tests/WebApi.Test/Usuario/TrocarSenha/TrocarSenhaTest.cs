using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Integracao.Test.Usuario.TrocarSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class TrocarSenhaTest : FfkApiClassFixture
{
    private readonly string _baseUrl = "usuario/trocar-senha";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioParaTrocarSenha;

    public TrocarSenhaTest()
    {
        _usuarioParaTrocarSenha = (FfkApi.Domain.Entities.Usuario)_factory.EntidadesCriadas()["UsuarioParaTrocarSenha"];
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioParaTrocarSenha.Id);

        var request = RequestTrocarSenhaBuilder.Build();
        request.SenhaAntiga = _usuarioParaTrocarSenha.Senha!;

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Erro_Nenhuma_Alteracao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioParaTrocarSenha.Id);

        var request = new RequestTrocarSenha()
        {
            SenhaAntiga = _usuarioParaTrocarSenha.Senha!,
            NovaSenha = _usuarioParaTrocarSenha.Senha!
        };

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("NENHUMA_ALTERACAO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestTrocarSenhaBuilder.Build();

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request);

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

        var request = RequestTrocarSenhaBuilder.Build();

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, "invalidToken");

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

        var request = RequestTrocarSenhaBuilder.Build();

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Nova_Senha_Vazia(string? senha)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioParaTrocarSenha.Id);

        var request = new RequestTrocarSenha()
        {
            SenhaAntiga = _usuarioParaTrocarSenha.Senha!,
            NovaSenha = senha
        };

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SENHA_VAZIA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nova_Senha_Invalida()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioParaTrocarSenha.Id);

        var request = new RequestTrocarSenha()
        {
            SenhaAntiga = _usuarioParaTrocarSenha.Senha!,
            NovaSenha = "SenhaInvalida"
        };

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SENHA_INVALIDA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Senha_Diferente_Da_Senha_Atual()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioParaTrocarSenha.Id);

        var request = RequestTrocarSenhaBuilder.Build();
        request.SenhaAntiga = _usuarioParaTrocarSenha.Senha + "1";

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SENHA_ANTIGA_DIFERENTE_DA_SENHA_INFORMADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioParaTrocarSenha.Id);

        var request = RequestTrocarSenhaBuilder.Build();
        request.SenhaAntiga = _usuarioParaTrocarSenha.Senha!;

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, token, addAppToken: false);

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

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioParaTrocarSenha.Id);

        var request = RequestTrocarSenhaBuilder.Build();
        request.SenhaAntiga = _usuarioParaTrocarSenha.Senha!;

        var response = await HttpHelper.DoPut(_baseUrl, cancellationToken, request, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
