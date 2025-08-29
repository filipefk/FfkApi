using FfkApi.Exceptions;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Aceitacao.Test.Indisponibilidade.Excluir;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EExcluirIndisponibilidadeTest : E2EClassFixture
{
    private readonly string _baseUrlIndisponibilidade = "indisponibilidade";

    [Test]
    public async Task Sucesso_Administrador_Excluindo_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();
        var usuarioOrganizacaoNova = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: organizacaoNova.Nome,
            ativar: true);

        var indisponibilidade = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioOrganizacaoNova,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Excluindo_Para_Mesma_Organizacao()
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

        var response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Excluindo_Para_Si_Mesmo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioNovo = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: _usuarioAdministrador.Organizacao.Nome,
            ativar: true);

        var indisponibilidade = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioNovo,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioNovo.Id);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Excluindo_Para_Outro_Usuario()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioNovo = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: _usuarioAdministrador.Organizacao.Nome,
            ativar: true);

        var indisponibilidade = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioNovo,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        usuarioNovo = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: _usuarioAdministrador.Organizacao.Nome,
            ativar: true);

        token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioNovo.Id);

        response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Indisponibilidades");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Excluindo_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroIndisponibilidades = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: _usuarioAdministrador.Organizacao.Nome,
            permissoes: ["Cadastro de Indisponibilidades"],
            ativar: true);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();
        var usuarioOrganizacaoNova = await CadastroHelper.CadastrarNovoUsuario(
            nomeOrganizacao: organizacaoNova.Nome,
            ativar: true);

        var indisponibilidade = await CadastroHelper.CadastrarNovaIndisponibilidade(
            usuario: usuarioOrganizacaoNova,
            dataInicial: DateOnly.FromDateTime(DateTime.Now),
            dataFinal: DateOnly.FromDateTime(DateTime.Now));

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroIndisponibilidades.Id);

        response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", token: token, cancellationToken: cancellationToken);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, "tokenInvalid");

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{id}", cancellationToken: cancellationToken, token: token);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", cancellationToken, token);

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{Guid.NewGuid()}", token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
