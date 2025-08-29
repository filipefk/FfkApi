using FfkApi.Exceptions;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Feed.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EAlterarFeedTest : E2EClassFixture
{
    private readonly string _baseUrlFeed = "feed";

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var feed = await CadastroHelper.CadastrarNovoFeed();
        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feed.Id.ToString();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroFeeds = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Feeds"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroFeeds.Id);

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();
        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feedNovo.Id.ToString();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();
        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feedNovo.Id.ToString();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Feeds");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feedNovo.Id.ToString();
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nome_Equipe_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var nomeEquipe = "Equipe Inexistente";

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feedNovo.Id.ToString();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [nomeEquipe];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("NOMES_DE_EQUIPES_NAO_ENCONTRADOS").Replace("{lista}", nomeEquipe);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nome_Equipe_Nao_Encontrado_Na_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novoFeed = await CadastroHelper.CadastrarNovoFeed();
        var novaOrganizacao = await CadastroHelper.CadastrarNovaOrganizacao();
        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: novaOrganizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = novoFeed.Id.ToString();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("NOMES_DE_EQUIPES_NAO_ENCONTRADOS").Replace("{lista}", novaEquipe.Nome);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Email_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var emailUsuario = "email.inexistente@dominio.com";

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();
        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feedNovo.Id.ToString();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [emailUsuario];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAILS_DE_USUARIOS_NAO_ENCONTRADOS").Replace("{lista}", emailUsuario);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Email_Usuario_Nao_Encontrado_Na_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();
        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feedNovo.Id.ToString();
        request.Organizacao = organizacaoNova.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAILS_DE_USUARIOS_NAO_ENCONTRADOS").Replace("{lista}", _usuarioSemPerfilNemPermissao.Email);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Data_Expiracao_Invalida()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var feedNovo = await CadastroHelper.CadastrarNovoFeed();
        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feedNovo.Id.ToString();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];
        request.ExpiraEm = "31/02/2023";

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DATA_EXPIRACAO_INVALIDA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarFeedBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, cancellationToken: cancellationToken, request: request);

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

        var request = RequestAlterarFeedBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, cancellationToken: cancellationToken, request: request, token: "TokenInvalido");

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

        var request = RequestAlterarFeedBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestAlterarFeedBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var erros = dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();

        var mensagemEsperada = MessagesException.GetString("TOKEN_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
        Assert.That(dadosDaResposta.RootElement.GetProperty("tokenEstaExpirado").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Erro_Feed_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("FEED_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("      ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = id;
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ID_VAZIO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = id;
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ID_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarFeedBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var request = RequestAlterarFeedBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
