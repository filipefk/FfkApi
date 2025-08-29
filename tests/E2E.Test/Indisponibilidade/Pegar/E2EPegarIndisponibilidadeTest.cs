using FfkApi.Exceptions;
using System.Net;
using System.Text.Json;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Aceitacao.Test.Indisponibilidade.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EPegarIndisponibilidadeTest : E2EClassFixture
{
    private readonly string _baseUrlIndisponibilidade = "indisponibilidade";

    private static void AssertDadosDaRespostaComIndisponibilidade(JsonDocument dadosDaResposta, FfkApi.Domain.Entities.Indisponibilidade indisponibilidade)
    {
        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(id, Is.EqualTo(indisponibilidade.Id.ToString()));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(descricao, Is.EqualTo(indisponibilidade.Descricao));

        var dataInicial = dadosDaResposta.RootElement.GetProperty("dataInicial").GetString();
        Assert.That(dataInicial, Is.EqualTo(indisponibilidade.DataInicial.ToString("dd/MM/yyyy")));

        var dataFinal = dadosDaResposta.RootElement.GetProperty("dataFinal").GetString();
        Assert.That(dataFinal, Is.EqualTo(indisponibilidade.DataFinal.ToString("dd/MM/yyyy")));

        var idUsuario = dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("id").GetString();
        Assert.That(idUsuario, Is.EqualTo(indisponibilidade.Usuario.Id.ToString()));

        var nomeUsuario = dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("nome").GetString();
        Assert.That(nomeUsuario, Is.EqualTo(indisponibilidade.Usuario.Nome));

        var emailUsuario = dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("email").GetString();
        Assert.That(emailUsuario, Is.EqualTo(indisponibilidade.Usuario.Email));

        var organizacaoUsuario = dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("organizacao").GetString();
        Assert.That(organizacaoUsuario, Is.EqualTo(indisponibilidade.Usuario.Organizacao.Nome));
    }

    [Test]
    public async Task Sucesso_Administrador_Pegar_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();
        var usuarioOrganizacaoNova = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: organizacaoNova.Nome,
            ativar: true);

        var indisponibilidade = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioOrganizacaoNova,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComIndisponibilidade(dadosDaResposta, indisponibilidade);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Pegar_Da_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroIndisponibilidades = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: _usuarioAdministrador.Organizacao.Nome,
            permissoes: ["Cadastro de Indisponibilidades"],
            ativar: true);

        var usuarioNovo = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: usuarioPermissaoCadastroIndisponibilidades.Organizacao.Nome,
            ativar: true);

        var indisponibilidade = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioNovo,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroIndisponibilidades.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComIndisponibilidade(dadosDaResposta, indisponibilidade);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Pegar_Da_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioNovo = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: _usuarioSemPerfilNemPermissao.Organizacao.Nome,
            ativar: true);

        var indisponibilidade = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioNovo,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComIndisponibilidade(dadosDaResposta, indisponibilidade);
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Pegar_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();
        var usuarioOrganizacaoNova = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: organizacaoNova.Nome,
            ativar: true);

        var indisponibilidadeOrganizacaoNova = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioOrganizacaoNova,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var usuarioPermissaoCadastroIndisponibilidades = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: _usuarioAdministrador.Organizacao.Nome,
            permissoes: ["Cadastro de Indisponibilidades"],
            ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{indisponibilidadeOrganizacaoNova.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComIndisponibilidade(dadosDaResposta, indisponibilidadeOrganizacaoNova);

        token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroIndisponibilidades.Id);

        response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{indisponibilidadeOrganizacaoNova.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("INDISPONIBILIDADE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Perfil_Nem_Permissao_Pegar_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();
        var usuarioOrganizacaoNova = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: organizacaoNova.Nome,
            ativar: true);

        var indisponibilidadeOrganizacaoNova = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioOrganizacaoNova,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{indisponibilidadeOrganizacaoNova.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        AssertDadosDaRespostaComIndisponibilidade(dadosDaResposta, indisponibilidadeOrganizacaoNova);

        token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{indisponibilidadeOrganizacaoNova.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("INDISPONIBILIDADE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Indisponibilidade_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("INDISPONIBILIDADE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, "tokenInvalid");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{id}", cancellationToken: cancellationToken, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ID_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_TOKEN");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_De_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(Guid.NewGuid());

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, token, addAppToken: false);

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

        var response = await HttpHelper.DoGet($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
