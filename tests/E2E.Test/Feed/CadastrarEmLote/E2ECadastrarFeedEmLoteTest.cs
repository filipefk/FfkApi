using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Enums;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using System.Net;
using System.Text.Json;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Feed.CadastrarEmLote;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2ECadastrarFeedEmLoteTest : E2EClassFixture
{
    private readonly string _baseUrlFeed = "feed";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private ResponseCadastrarEmLote<RequestCadastrarFeed, ResponseDadosFeed> DeserializaDadosDaResposta(JsonDocument jsonDocument)
    {
        return jsonDocument.RootElement.Deserialize<ResponseCadastrarEmLote<RequestCadastrarFeed, ResponseDadosFeed>>(
            _jsonSerializerOptions)!;
    }

    private static void CompararRequests(RequestCadastrarFeed requestCadastrarFeed1, RequestCadastrarFeed requestCadastrarFeed2)
    {
        Assert.That(requestCadastrarFeed1.Nome, Is.EqualTo(requestCadastrarFeed2.Nome));
        Assert.That(requestCadastrarFeed1.Descricao, Is.EqualTo(requestCadastrarFeed2.Descricao));
        Assert.That(requestCadastrarFeed1.PalavrasChave, Is.EqualTo(requestCadastrarFeed2.PalavrasChave));
        Assert.That(requestCadastrarFeed1.Status, Is.EqualTo(requestCadastrarFeed2.Status));
        Assert.That(requestCadastrarFeed1.VisibilidadeUsuarios, Is.EquivalentTo(requestCadastrarFeed2.VisibilidadeUsuarios!));
        Assert.That(requestCadastrarFeed1.VisibilidadeEquipes, Is.EquivalentTo(requestCadastrarFeed2.VisibilidadeEquipes!));
        Assert.That(requestCadastrarFeed1.ExpiraEm, Is.EqualTo(requestCadastrarFeed2.ExpiraEm));
        Assert.That(requestCadastrarFeed1.Organizacao, Is.EqualTo(requestCadastrarFeed2.Organizacao));
    }

    private static void CompararRequestComResponse(RequestCadastrarFeed requestCadastrarFeed, ResponseDadosFeed responseDadosFeed)
    {
        Assert.That(requestCadastrarFeed.Nome, Is.EqualTo(responseDadosFeed.Nome));
        Assert.That(requestCadastrarFeed.Descricao, Is.EqualTo(responseDadosFeed.Descricao));
        Assert.That(requestCadastrarFeed.PalavrasChave, Is.EqualTo(responseDadosFeed.PalavrasChave));
        Assert.That(requestCadastrarFeed.Status, Is.EqualTo(responseDadosFeed.Status));
        Assert.That(requestCadastrarFeed.VisibilidadeUsuarios, Is.EquivalentTo(responseDadosFeed.VisibilidadeUsuarios));
        Assert.That(requestCadastrarFeed.VisibilidadeEquipes, Is.EquivalentTo(responseDadosFeed.VisibilidadeEquipes));
        Assert.That(requestCadastrarFeed.ExpiraEm, Is.EqualTo(responseDadosFeed.ExpiraEm));
        Assert.That(requestCadastrarFeed.Organizacao, Is.EqualTo(responseDadosFeed.Organizacao));
        Assert.That(responseDadosFeed.Anexos, Is.Empty);
    }

    [Test]
    public async Task Sucesso_Total()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = RequestCadastrarFeedEmLoteBuilder.Build(4);
        foreach (var feed in request.Feeds)
        {
            feed.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        }

        request.Feeds[0].VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];
        request.Feeds[0].VisibilidadeEquipes = [novaEquipe.Nome];

        request.Feeds[1].VisibilidadeUsuarios = [];
        request.Feeds[1].VisibilidadeEquipes = [novaEquipe.Nome];

        request.Feeds[2].VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];
        request.Feeds[2].VisibilidadeEquipes = [];

        for (int i = 3; i < request.Feeds.Count; i++)
        {
            request.Feeds[i].VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];
            request.Feeds[i].VisibilidadeEquipes = [novaEquipe.Nome];
        }

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj.Cadastrados.Count, Is.EqualTo(request.Feeds.Count));

        var dicionario = responseObj.Cadastrados
            .ToDictionary(
                c => c.Nome,
                c => c
            );

        foreach (var feedRequest in request.Feeds)
        {
            Assert.That(dicionario.ContainsKey(feedRequest.Nome!));
            CompararRequestComResponse(feedRequest, dicionario[feedRequest.Nome!]);
        }

        Assert.That(responseObj.Erros, Is.Empty);
        Assert.That(responseObj.TotalCadastrados, Is.EqualTo(request.Feeds.Count));
        Assert.That(responseObj.TotalErros, Is.EqualTo(0));
        Assert.That(responseObj.Resultado, Is.EqualTo(StatusCadastroLote.SucessoTotal.ToString()));
    }

    [Test]
    public async Task Sucesso_Parcial()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var novaEquipe = await CadastroHelper.CadastrarNovaEquipe(
            organizacao: _usuarioAdministrador.Organizacao,
            usuariosEquipe: []);

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = RequestCadastrarFeedEmLoteBuilder.Build(4);
        foreach (var feed in request.Feeds)
        {
            feed.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        }

        var nomeEquipe = "Equipe Inexistente";

        request.Feeds[0].VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];
        request.Feeds[0].VisibilidadeEquipes = [nomeEquipe];

        request.Feeds[1].VisibilidadeUsuarios = [];
        request.Feeds[1].VisibilidadeEquipes = [novaEquipe.Nome];

        request.Feeds[2].VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];
        request.Feeds[2].VisibilidadeEquipes = [];

        for (int i = 3; i < request.Feeds.Count; i++)
        {
            request.Feeds[i].VisibilidadeUsuarios = [_usuarioSemPerfilNemPermissao.Email];
            request.Feeds[i].VisibilidadeEquipes = [novaEquipe.Nome];
        }

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj.Cadastrados.Count, Is.EqualTo(request.Feeds.Count - 1));

        var dicionario = responseObj.Cadastrados
            .ToDictionary(
                c => c.Nome,
                c => c
            );

        for (int i = 1; i < request.Feeds.Count; i++)
        {
            var feedRequest = request.Feeds[i];
            Assert.That(dicionario.ContainsKey(feedRequest.Nome!));
            CompararRequestComResponse(feedRequest, dicionario[feedRequest.Nome!]);
        }

        Assert.That(responseObj.Erros.Count, Is.EqualTo(1));

        CompararRequests(responseObj.Erros[0].Request, request.Feeds[0]);

        Assert.That(responseObj.Erros[0].MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("NOME_EQUIPE_NAO_ENCONTRADO").Replace("{nome-equipe}", nomeEquipe)));
        Assert.That(responseObj.TotalCadastrados, Is.EqualTo(request.Feeds.Count - 1));
        Assert.That(responseObj.TotalErros, Is.EqualTo(1));
        Assert.That(responseObj.Resultado, Is.EqualTo(StatusCadastroLote.SucessoParcial.ToString()));
    }

    [Test]
    public async Task Falha_Total()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = RequestCadastrarFeedEmLoteBuilder.Build(4);
        var nomeOrganizacao = _usuarioAdministrador.Organizacao.Nome;
        foreach (var feed in request.Feeds)
        {
            feed.Organizacao = nomeOrganizacao;
            feed.VisibilidadeUsuarios = [];
            feed.VisibilidadeEquipes = [];
        }

        var requestOrganizacaoNull = request.Feeds.FirstOrDefault();
        requestOrganizacaoNull!.Organizacao = null;

        var organizacaoInvalida = "Inválida";
        var requestOrganizacaoNaoEncontrada = request.Feeds.Skip(1).FirstOrDefault();
        requestOrganizacaoNaoEncontrada!.Organizacao = organizacaoInvalida;

        var equipeInvalida = "Inválida";
        var requestEquipeInvalida = request.Feeds.Skip(2).FirstOrDefault();
        requestEquipeInvalida!.VisibilidadeEquipes = [equipeInvalida];

        var emailUsuarioInvalido = "emailinvalido@dominio.com";
        var requestEmailUsuarioNaoEncontrado = request.Feeds.Skip(3).FirstOrDefault();
        requestEmailUsuarioNaoEncontrado!.VisibilidadeUsuarios = [emailUsuarioInvalido];

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj!.Cadastrados.Count, Is.EqualTo(0));
        Assert.That(responseObj.Erros.Count, Is.EqualTo(request.Feeds.Count));

        var responseErroQueRequestTemOrganizacaoNull = responseObj.Erros
            .FirstOrDefault(e => string.IsNullOrWhiteSpace(e.Request.Organizacao));

        CompararRequests(responseErroQueRequestTemOrganizacaoNull!.Request, requestOrganizacaoNull);
        Assert.That(responseErroQueRequestTemOrganizacaoNull.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("ORGANIZACAO_VAZIA")));

        var responseErroQueRequestTemOrganizacaoNaoEncontrada = responseObj.Erros
            .FirstOrDefault(e => e.Request.Organizacao == organizacaoInvalida);

        CompararRequests(responseErroQueRequestTemOrganizacaoNaoEncontrada!.Request, requestOrganizacaoNaoEncontrada);
        Assert.That(responseErroQueRequestTemOrganizacaoNaoEncontrada.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA")));

        var responseErroQueRequestTemEquipeInvalida = responseObj.Erros
            .FirstOrDefault(e => e.Request.VisibilidadeEquipes!.FirstOrDefault() == equipeInvalida);

        CompararRequests(responseErroQueRequestTemEquipeInvalida!.Request, requestEquipeInvalida!);
        Assert.That(responseErroQueRequestTemEquipeInvalida.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("NOME_EQUIPE_NAO_ENCONTRADO").Replace("{nome-equipe}", equipeInvalida)));


        var responseErroQueRequestTemEmailUsuarioNaoEncontrado = responseObj.Erros
            .FirstOrDefault(e => e.Request.VisibilidadeUsuarios!.FirstOrDefault() == emailUsuarioInvalido);

        CompararRequests(responseErroQueRequestTemEmailUsuarioNaoEncontrado!.Request, requestEmailUsuarioNaoEncontrado!);
        Assert.That(responseErroQueRequestTemEmailUsuarioNaoEncontrado.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("EMAIL_USUARIO_NAO_ENCONTRADO").Replace("{email}", emailUsuarioInvalido)));

        Assert.That(responseObj.TotalCadastrados, Is.EqualTo(0));
        Assert.That(responseObj.TotalErros, Is.EqualTo(request.Feeds.Count));
        Assert.That(responseObj.Resultado, Is.EqualTo(StatusCadastroLote.Falha.ToString()));
    }

    [Test]
    public async Task Erro_Lista_Feeds_Vazia()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = new RequestCadastrarFeedEmLote
        {
            Feeds = []
        };

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj.Cadastrados, Is.Empty);
        Assert.That(responseObj.Erros.Count, Is.EqualTo(1));
        Assert.That(responseObj.Erros[0].Request, Is.Null);
        Assert.That(responseObj.Erros[0].MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("LISTA_DE_FEED_VAZIA")));
    }

    [Test]
    public async Task Erro_Usando_Token_De_Usuario()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarFeedEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_INVALIDO")));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("SEM_TOKEN")));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarFeedEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: "TokenInvalido", cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_INVALIDO")));
    }

    [Test]
    public async Task Erro_Token_De_Sistema_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(Guid.NewGuid());

        var request = RequestCadastrarFeedEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_INVALIDO")));
    }

    [Test]
    public async Task Erro_Token_Expirado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2Ns"
                    + "YWltcy9zaWQiOiIwYmY3ZTNmNC0zMTNjLTQ5YzYtODhhZS1jODNkZDFiMGI5YjAiLCJuYmYiOjE3NDg2MTMxMjUsImV4cCI6MTc0ODY"
                    + "xMzE4NSwiaWF0IjoxNzQ4NjEzMTI1fQ.pyoO-CcPlhLlCGuEIHXa0boyL4XHXzF3apRHw6wZdks";

        var request = RequestCadastrarFeedEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_EXPIRADO")));
        Assert.That(erros.TokenEstaExpirado);
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarFeedBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_APLICACAO_AUSENTE")));
    }

    [Test]
    public async Task Erro_App_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = RequestCadastrarFeedEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlFeed}/lote", request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_APLICACAO_INVALIDO")));
    }
}
