using FfkApi.Exceptions;
using System.Net;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Feed.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2ECadastrarFeedTest : E2EClassFixture
{
    private readonly string _baseUrlFeed = "feed";

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var palavrasChave = dadosDaResposta.RootElement.GetProperty("palavrasChave").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(palavrasChave));
        Assert.That(palavrasChave, Is.EqualTo(request.PalavrasChave));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(request.Status));

        var anexos = dadosDaResposta.RootElement.GetProperty("anexos").EnumerateArray();
        Assert.That(anexos, Is.Not.Null);
        Assert.That(anexos, Is.Empty);

        var visibilidadeUsuarios = dadosDaResposta.RootElement.GetProperty("visibilidadeUsuarios").EnumerateArray();
        Assert.That(visibilidadeUsuarios, Is.Not.Null);
        Assert.That(visibilidadeUsuarios.ToListString(), Is.EquivalentTo(request.VisibilidadeUsuarios!));

        var visibilidadeEquipes = dadosDaResposta.RootElement.GetProperty("visibilidadeEquipes").EnumerateArray();
        Assert.That(visibilidadeEquipes, Is.Not.Null);
        Assert.That(visibilidadeEquipes.ToListString(), Is.EquivalentTo(request.VisibilidadeEquipes!));

        var expiraEm = dadosDaResposta.RootElement.GetProperty("expiraEm").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(expiraEm));
        Assert.That(expiraEm, Is.EqualTo(request.ExpiraEm));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(request.Organizacao));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroFeeds = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Feeds"], ativar: true);

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroFeeds.Id);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(descricao));
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var palavrasChave = dadosDaResposta.RootElement.GetProperty("palavrasChave").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(palavrasChave));
        Assert.That(palavrasChave, Is.EqualTo(request.PalavrasChave));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(request.Status));

        var visibilidadeUsuarios = dadosDaResposta.RootElement.GetProperty("visibilidadeUsuarios").EnumerateArray();
        Assert.That(visibilidadeUsuarios, Is.Not.Null);
        Assert.That(visibilidadeUsuarios.ToListString(), Is.EquivalentTo(request.VisibilidadeUsuarios!));

        var visibilidadeEquipes = dadosDaResposta.RootElement.GetProperty("visibilidadeEquipes").EnumerateArray();
        Assert.That(visibilidadeEquipes, Is.Not.Null);
        Assert.That(visibilidadeEquipes.ToListString(), Is.EquivalentTo(request.VisibilidadeEquipes!));

        var expiraEm = dadosDaResposta.RootElement.GetProperty("expiraEm").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(expiraEm));
        Assert.That(expiraEm, Is.EqualTo(request.ExpiraEm));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(request.Organizacao));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

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

        var request = RequestCadastrarFeedBuilder.Build();
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

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

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [nomeEquipe];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

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

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();
        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = organizacaoNova.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

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

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [emailUsuario];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

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

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();
        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: organizacaoNova,
            usuariosEquipe: []);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = organizacaoNova.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAILS_DE_USUARIOS_NAO_ENCONTRADOS").Replace("{lista}", _usuarioSemPerfilNemPermissao.Email);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Email_Usuario_Vazio()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [novaEquipe.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email, ""];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAIL_USUARIO_VAZIO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, cancellationToken: cancellationToken);

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

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, cancellationToken: cancellationToken, request: request, token: "TokenInvalido");

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

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var erros = dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();

        var mensagemEsperada = MessagesException.GetString("TOKEN_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
        Assert.That(dadosDaResposta.RootElement.GetProperty("tokenEstaExpirado").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
