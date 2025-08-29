using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;

namespace Integracao.Test.Usuario.Ativar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AtivarUsuarioTest : FfkApiClassFixture
{
    private readonly string _baseUrlUsuarioAtivar = "usuario/Ativar";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioInativoComTokenAtivacaoValido;
    private readonly FfkApi.Domain.Entities.TokenAtivacao _tokenAtivacaoValido;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioInativoComTokenAtivacaoVencido;
    private readonly FfkApi.Domain.Entities.TokenAtivacao _tokenAtivacaoVencido;

    public AtivarUsuarioTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioInativoComTokenAtivacaoValido = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioInativoComTokenAtivacaoValido"];
        _tokenAtivacaoValido = (FfkApi.Domain.Entities.TokenAtivacao)entidades["TokenAtivacaoValido"];
        _usuarioInativoComTokenAtivacaoVencido = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioInativoComTokenAtivacaoVencido"];
        _tokenAtivacaoVencido = (FfkApi.Domain.Entities.TokenAtivacao)entidades["TokenAtivacaoVencido"];
    }

    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoValido, _tokenAtivacaoValido.Valor);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Erro_Token_Ativacao_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoValido);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_ATIVACAO_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Ativacao_Vencido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoVencido, _tokenAtivacaoVencido.Valor);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_ATIVACAO_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Dados_Invalidos_Nome()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoValido, _tokenAtivacaoValido.Valor);
        request.Nome = "NomeErrado";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request);

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

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoValido, _tokenAtivacaoValido.Valor);
        request.Email = "emailerrado@provedor.com";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request);

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

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoValido, _tokenAtivacaoValido.Valor);
        request.Cpf = "81289663084";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request);

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

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoValido, _tokenAtivacaoValido.Valor);
        request.Senha = "senhainvalida";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request);

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

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoValido, _tokenAtivacaoValido.Valor);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request, addAppToken: false);

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

        var request = RequestAtivarUsuarioBuilder.Build(_usuarioInativoComTokenAtivacaoValido, _tokenAtivacaoValido.Valor);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuarioAtivar, cancellationToken: cancellationToken, request: request, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
