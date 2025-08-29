using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;

namespace Integracao.Test.Usuario.NovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class NovaSenhaUsuarioTest : FfkApiClassFixture
{
    private readonly string _baseUrlUsuarioNovaSenha = "usuario/nova-senha";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAtivoComTokenNovaSenhaValido;
    private readonly FfkApi.Domain.Entities.TokenNovaSenha _tokenNovaSenhaValido;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAtivoComTokenNovaSenhaVencido;
    private readonly FfkApi.Domain.Entities.TokenNovaSenha _tokenNovaSenhaVencido;

    public NovaSenhaUsuarioTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAtivoComTokenNovaSenhaValido = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAtivoComTokenNovaSenhaValido"];
        _tokenNovaSenhaValido = (FfkApi.Domain.Entities.TokenNovaSenha)entidades["TokenNovaSenhaValido"];
        _usuarioAtivoComTokenNovaSenhaVencido = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAtivoComTokenNovaSenhaVencido"];
        _tokenNovaSenhaVencido = (FfkApi.Domain.Entities.TokenNovaSenha)entidades["TokenNovaSenhaVencido"];
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaValido, _tokenNovaSenhaValido.Valor);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Erro_Token_Nova_Senha_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaValido);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_NOVA_SENHA_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Nova_Senha_Vencido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaVencido, _tokenNovaSenhaVencido.Valor);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_NOVA_SENHA_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Dados_Invalidos_Nome()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaValido, _tokenNovaSenhaValido.Valor);
        request.Nome = "NomeErrado";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DADOS_INVALIDOS");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Dados_Invalidos_Email()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaValido, _tokenNovaSenhaValido.Valor);
        request.Email = "emailerrado@provedor.com";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request);

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

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaValido, _tokenNovaSenhaValido.Valor);
        request.Cpf = "81289663084";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DADOS_INVALIDOS");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Senha_Invalida()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaValido, _tokenNovaSenhaValido.Valor);
        request.NovaSenha = "senhainvalida";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SENHA_INVALIDA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaValido, _tokenNovaSenhaValido.Valor);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request, addAppToken: false);

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

        var request = RequestNovaSenhaUsuarioBuilder.Build(_usuarioAtivoComTokenNovaSenhaValido, _tokenNovaSenhaValido.Valor);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioNovaSenha, cancellationToken: cancellationToken, request: request, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
