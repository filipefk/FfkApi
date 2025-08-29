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

namespace Aceitacao.Test.Organizacao.CadastrarEmLote;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2ECadastrarOrganizacaoEmLoteTest : E2EClassFixture
{
    private readonly string _baseUrlOrganizacao = "organizacao";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private ResponseCadastrarEmLote<RequestCadastrarOrganizacao, ResponseDadosOrganizacao> DeserializaDadosDaResposta(JsonDocument jsonDocument)
    {
        return jsonDocument.RootElement.Deserialize<ResponseCadastrarEmLote<RequestCadastrarOrganizacao, ResponseDadosOrganizacao>>(
            _jsonSerializerOptions)!;
    }

    private static void CompararRequests(RequestCadastrarOrganizacao requestCadastrarOrganizacao1, RequestCadastrarOrganizacao requestCadastrarOrganizacao2)
    {
        Assert.That(requestCadastrarOrganizacao1.Nome, Is.EqualTo(requestCadastrarOrganizacao2.Nome));
        Assert.That(requestCadastrarOrganizacao1.Descricao, Is.EqualTo(requestCadastrarOrganizacao2.Descricao));
        Assert.That(requestCadastrarOrganizacao1.RemetenteEmail, Is.EqualTo(requestCadastrarOrganizacao2.RemetenteEmail));
        Assert.That(requestCadastrarOrganizacao1.RemetenteNome, Is.EqualTo(requestCadastrarOrganizacao2.RemetenteNome));
        Assert.That(requestCadastrarOrganizacao1.ModeloEmailAtivacao, Is.EqualTo(requestCadastrarOrganizacao2.ModeloEmailAtivacao));
        Assert.That(requestCadastrarOrganizacao1.ModeloEmailNovaSenha, Is.EqualTo(requestCadastrarOrganizacao2.ModeloEmailNovaSenha));
    }

    private static void CompararRequestComResponse(RequestCadastrarOrganizacao requestCadastrarOrganizacao, ResponseDadosOrganizacao responseDadosOrganizacao)
    {
        Assert.That(requestCadastrarOrganizacao.Nome, Is.EqualTo(responseDadosOrganizacao.Nome));
        Assert.That(requestCadastrarOrganizacao.Descricao, Is.EqualTo(responseDadosOrganizacao.Descricao));
        Assert.That(requestCadastrarOrganizacao.RemetenteEmail, Is.EqualTo(responseDadosOrganizacao.RemetenteEmail));
        Assert.That(requestCadastrarOrganizacao.RemetenteNome, Is.EqualTo(responseDadosOrganizacao.RemetenteNome));
        Assert.That(requestCadastrarOrganizacao.ModeloEmailAtivacao, Is.EqualTo(responseDadosOrganizacao.ModeloEmailAtivacao));
        Assert.That(requestCadastrarOrganizacao.ModeloEmailNovaSenha, Is.EqualTo(responseDadosOrganizacao.ModeloEmailNovaSenha));
    }

    [Test]
    public async Task Sucesso_Total()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj.Cadastrados.Count, Is.EqualTo(request.Organizacoes.Count));

        var dicionario = responseObj.Cadastrados
            .ToDictionary(
                c => c.Nome,
                c => c
            );

        foreach (var atoRequest in request.Organizacoes)
        {
            Assert.That(dicionario.ContainsKey(atoRequest.Nome!));
            CompararRequestComResponse(atoRequest, dicionario[atoRequest.Nome!]);
        }

        Assert.That(responseObj.Erros, Is.Empty);
        Assert.That(responseObj.TotalCadastrados, Is.EqualTo(request.Organizacoes.Count));
        Assert.That(responseObj.TotalErros, Is.EqualTo(0));
        Assert.That(responseObj.Resultado, Is.EqualTo(StatusCadastroLote.SucessoTotal.ToString()));
    }

    [Test]
    public async Task Sucesso_Parcial()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();
        var requestNomeNull = request.Organizacoes.FirstOrDefault();
        requestNomeNull!.Nome = null;

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj.Cadastrados.Count, Is.EqualTo(request.Organizacoes.Count - 1));

        var dicionario = responseObj.Cadastrados
            .ToDictionary(
                c => c.Nome,
                c => c
            );

        foreach (var atoRequest in request.Organizacoes.Where(o => o.Nome != null))
        {
            Assert.That(dicionario.ContainsKey(atoRequest.Nome!));
            CompararRequestComResponse(atoRequest, dicionario[atoRequest.Nome!]);
        }

        Assert.That(responseObj.Erros.Count, Is.EqualTo(1));

        CompararRequests(responseObj.Erros[0].Request, requestNomeNull);

        Assert.That(responseObj.Erros[0].MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("NOME_VAZIO")));
        Assert.That(responseObj.TotalCadastrados, Is.EqualTo(request.Organizacoes.Count - 1));
        Assert.That(responseObj.TotalErros, Is.EqualTo(1));
        Assert.That(responseObj.Resultado, Is.EqualTo(StatusCadastroLote.SucessoParcial.ToString()));
    }

    [Test]
    public async Task Falha_Total()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var organizacaoJaExistente = await CadastroHelper.CadastrarNovaOrganizacao();

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build(2);

        var requestNomeJaExiste = request.Organizacoes.FirstOrDefault();
        requestNomeJaExiste!.Nome = organizacaoJaExistente.Nome;

        var requestDescricaoNull = request.Organizacoes.Skip(1).FirstOrDefault();
        requestDescricaoNull!.Descricao = null;

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj!.Cadastrados.Count, Is.EqualTo(0));
        Assert.That(responseObj.Erros.Count, Is.EqualTo(request.Organizacoes.Count));

        var responseErroQueRequestNomeJaExiste = responseObj.Erros
            .FirstOrDefault(e => e.Request.Nome == organizacaoJaExistente.Nome);

        CompararRequests(responseErroQueRequestNomeJaExiste!.Request, requestNomeJaExiste);
        Assert.That(responseErroQueRequestNomeJaExiste.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("NOME_DE_ORGANIZACAO_JA_EXISTE")));

        var responseErroQueRequestDescricaoNull = responseObj.Erros
            .FirstOrDefault(e => string.IsNullOrWhiteSpace(e.Request.Descricao));

        CompararRequests(responseErroQueRequestDescricaoNull!.Request, requestDescricaoNull);
        Assert.That(responseErroQueRequestDescricaoNull.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("DESCRICAO_VAZIA")));

        Assert.That(responseObj.TotalCadastrados, Is.EqualTo(0));
        Assert.That(responseObj.TotalErros, Is.EqualTo(request.Organizacoes.Count));
        Assert.That(responseObj.Resultado, Is.EqualTo(StatusCadastroLote.Falha.ToString()));
    }

    [Test]
    public async Task Erro_Nome_Organizacao_Repetido_Na_Lista()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build(2);
        request.Organizacoes[1].Nome = request.Organizacoes[0].Nome;

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj.Cadastrados.Count, Is.EqualTo(1));
        Assert.That(responseObj.Erros.Count, Is.EqualTo(1));
        Assert.That(responseObj.Erros[0].MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("NOME_DE_ORGANIZACAO_REPETIDO")));
        Assert.That(responseObj.TotalCadastrados, Is.EqualTo(1));
        Assert.That(responseObj.TotalErros, Is.EqualTo(1));
        Assert.That(responseObj.Resultado, Is.EqualTo(StatusCadastroLote.SucessoParcial.ToString()));
    }

    [Test]
    public async Task Erro_Lista_Organizacoes_Vazia()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var sistemaCliente = await CadastroHelper.CadastrarNovoSistemaCliente();

        var token = GeradorTokenSistemaClienteBuilder.Build().Gerar(sistemaCliente.Id);

        var request = new RequestCadastrarOrganizacaoEmLote
        {
            Organizacoes = []
        };

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var responseObj = DeserializaDadosDaResposta(await HttpResponseUtil.PegarDadosDaResposta(response));

        Assert.That(responseObj.Cadastrados, Is.Empty);
        Assert.That(responseObj.Erros.Count, Is.EqualTo(1));
        Assert.That(responseObj.Erros[0].Request, Is.Null);
        Assert.That(responseObj.Erros[0].MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("LISTA_DE_ORGANIZACAO_VAZIA")));
    }

    [Test]
    public async Task Erro_Usando_Token_De_Usuario()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_INVALIDO")));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("SEM_TOKEN")));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: "TokenInvalido", cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_INVALIDO")));
    }

    [Test]
    public async Task Erro_Token_De_Sistema_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(Guid.NewGuid());

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken);

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

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken);

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

        var request = RequestCadastrarOrganizacaoBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var request = RequestCadastrarOrganizacaoEmLoteBuilder.Build();

        var response = await HttpHelper.DoPost(url: $"{_baseUrlOrganizacao}/lote", request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.ConverterParaResponseErro(response);
        Assert.That(erros.MensagensDeErro.ListaStringTemSoUmItem(MessagesException.GetString("TOKEN_APLICACAO_INVALIDO")));
    }
}
