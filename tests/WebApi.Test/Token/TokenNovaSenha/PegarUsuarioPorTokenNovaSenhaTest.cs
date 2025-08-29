using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;

namespace Integracao.Test.Token.TokenNovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioPorTokenNovaSenhaTest : FfkApiClassFixture
{
    private readonly string _baseUrlTokenNovaSenha = "token/nova-senha";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAtivoComTokenNovaSenhaValido;
    private readonly FfkApi.Domain.Entities.TokenNovaSenha _tokenNovaSenhaValido;
    private readonly FfkApi.Domain.Entities.TokenNovaSenha _tokenNovaSenhaVencido;

    public PegarUsuarioPorTokenNovaSenhaTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAtivoComTokenNovaSenhaValido = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAtivoComTokenNovaSenhaValido"];
        _tokenNovaSenhaValido = (FfkApi.Domain.Entities.TokenNovaSenha)entidades["TokenNovaSenhaValido"];
        _tokenNovaSenhaVencido = (FfkApi.Domain.Entities.TokenNovaSenha)entidades["TokenNovaSenhaVencido"];
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlTokenNovaSenha}/{_tokenNovaSenhaValido.Valor}", cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(_usuarioAtivoComTokenNovaSenhaValido.Nome));
    }

    [Test]
    public async Task Erro_Token_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlTokenNovaSenha}/umlixoqualqueraqui", cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_NOVA_SENHA_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Vencido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlTokenNovaSenha}/{_tokenNovaSenhaVencido.Valor}", cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_NOVA_SENHA_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlTokenNovaSenha}/{_tokenNovaSenhaValido.Valor}", cancellationToken, addAppToken: false);

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

        var response = await HttpHelper.DoGet($"{_baseUrlTokenNovaSenha}/{_tokenNovaSenhaValido.Valor}", cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

}
