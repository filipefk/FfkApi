using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;

namespace Integracao.Test.Token.TokenNovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class NovoTokenNovaSenhaTest : FfkApiClassFixture
{
    private readonly string _baseUrlTokenNovaSenha = "token/nova-senha";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioInativoComTokenAtivacaoValido;

    public NovoTokenNovaSenhaTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _usuarioInativoComTokenAtivacaoValido = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioInativoComTokenAtivacaoValido"];
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovoTokenNovaSenhaBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPost(url: _baseUrlTokenNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(_usuarioSemPerfilNemPermissao.Nome));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovoTokenNovaSenhaBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Email = "emailerrado@provedor.com";

        var response = await HttpHelper.DoPost(url: _baseUrlTokenNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Inativo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovoTokenNovaSenhaBuilder.Build(_usuarioInativoComTokenAtivacaoValido);

        var response = await HttpHelper.DoPost(url: _baseUrlTokenNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Dados_Invalidos_Nome()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovoTokenNovaSenhaBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Nome = "NomeErrado";

        var response = await HttpHelper.DoPost(url: _baseUrlTokenNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DADOS_INVALIDOS");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Dados_Invalidos_Cpf()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovoTokenNovaSenhaBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Cpf = "53259182063";

        var response = await HttpHelper.DoPost(url: _baseUrlTokenNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DADOS_INVALIDOS");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Cpf_Vazio(string cpf)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovoTokenNovaSenhaBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Cpf = cpf;

        var response = await HttpHelper.DoPost(url: _baseUrlTokenNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("CPF_VAZIO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovoTokenNovaSenhaBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPost(url: _baseUrlTokenNovaSenha, cancellationToken: cancellationToken, request: request, addAppToken: false);

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

        var request = RequestNovoTokenNovaSenhaBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPost(url: _baseUrlTokenNovaSenha, cancellationToken: cancellationToken, request: request, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
