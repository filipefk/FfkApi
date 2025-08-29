using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;

namespace Integracao.Test.Token.TokenAtivacao;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioPorTokenAtivacaoTest : FfkApiClassFixture
{
    private readonly string _baseUrlTokenAtivacao = "token/ativacao";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioInativoComTokenAtivacaoValido;
    private readonly FfkApi.Domain.Entities.TokenAtivacao _tokenAtivacaoValido;
    private readonly FfkApi.Domain.Entities.TokenAtivacao _tokenAtivacaoVencido;

    public PegarUsuarioPorTokenAtivacaoTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioInativoComTokenAtivacaoValido = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioInativoComTokenAtivacaoValido"];
        _tokenAtivacaoValido = (FfkApi.Domain.Entities.TokenAtivacao)entidades["TokenAtivacaoValido"];
        _tokenAtivacaoVencido = (FfkApi.Domain.Entities.TokenAtivacao)entidades["TokenAtivacaoVencido"];
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlTokenAtivacao}/{_tokenAtivacaoValido.Valor}", cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(_usuarioInativoComTokenAtivacaoValido.Nome));
    }

    [Test]
    public async Task Erro_Token_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlTokenAtivacao}/umlixoqualqueraqui", cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_ATIVACAO_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Vencido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlTokenAtivacao}/{_tokenAtivacaoVencido.Valor}", cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_ATIVACAO_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlTokenAtivacao}/{_tokenAtivacaoValido.Valor}", cancellationToken, addAppToken: false);

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

        var response = await HttpHelper.DoGet($"{_baseUrlTokenAtivacao}/{_tokenAtivacaoValido.Valor}", cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

}
