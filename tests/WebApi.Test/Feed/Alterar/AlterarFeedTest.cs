using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Integracao.Test.Feed.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarFeedTest : FfkApiClassFixture
{
    private readonly string _baseUrlFeed = "feed";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroFeeds;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Feed _feedNovo;
    private readonly FfkApi.Domain.Entities.Organizacao _organizacaoFfkApi;
    private readonly FfkApi.Domain.Entities.Organizacao _organizacaoNova;
    private readonly FfkApi.Domain.Entities.Equipe _equipeNova;

    public AlterarFeedTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroFeeds = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroFeeds"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _feedNovo = (FfkApi.Domain.Entities.Feed)entidades["FeedNovo"];
        _organizacaoFfkApi = (FfkApi.Domain.Entities.Organizacao)entidades["OrganizacaoFfkApi"];
        _organizacaoNova = (FfkApi.Domain.Entities.Organizacao)entidades["OrganizacaoNova"];
        _equipeNova = (FfkApi.Domain.Entities.Equipe)entidades["EquipeNova"];
    }

    private async Task<FfkApi.Domain.Entities.Feed> CadastrarNovoFeed()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarFeedBuilder.Build();
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [];
        request.VisibilidadeUsuarios = [];

        var response = await HttpHelper.DoPost(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

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

        return new FfkApi.Domain.Entities.Feed
        {
            Id = Guid.Parse(id!),
            Nome = nome!,
            Descricao = descricao!,
            PalavrasChave = palavrasChave!,
            Status = EnumUtil.ConverterTextoParaEnum<StatusFeed>(status!),
            VisibilidadeUsuarios = [],
            VisibilidadeEquipes = [],
            ExpiraEm = DateOnly.ParseExact(expiraEm!, "dd/MM/yyyy"),
            IdOrganizacao = _organizacaoFfkApi.Id,
            Organizacao = _organizacaoFfkApi
        };
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var feed = await CadastrarNovoFeed();

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feed.Id.ToString();
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroFeeds.Id);

        var feed = await CadastrarNovoFeed();

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = feed.Id.ToString();
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
        request.VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = _feedNovo.Id.ToString();
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
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

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = _feedNovo.Id.ToString();
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

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = _feedNovo.Id.ToString();
        request.Organizacao = _organizacaoFfkApi.Nome;
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

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = _feedNovo.Id.ToString();
        request.Organizacao = _organizacaoNova.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
        request.VisibilidadeUsuarios = [];

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("NOMES_DE_EQUIPES_NAO_ENCONTRADOS").Replace("{lista}", _equipeNova.Nome);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Email_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var emailUsuario = "email.inexistente@dominio.com";

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = _feedNovo.Id.ToString();
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
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

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = _feedNovo.Id.ToString();
        request.Organizacao = _organizacaoNova.Nome;
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

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = _feedNovo.Id.ToString();
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
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

        var request = RequestAlterarFeedBuilder.Build();
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
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

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = id;
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
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

        var request = RequestAlterarFeedBuilder.Build();
        request.Id = id;
        request.Organizacao = _organizacaoFfkApi.Nome;
        request.VisibilidadeEquipes = [_equipeNova.Nome];
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
        request.Id = _feedNovo.Id.ToString();

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
        request.Id = _feedNovo.Id.ToString();

        var response = await HttpHelper.DoPut(url: _baseUrlFeed, request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
