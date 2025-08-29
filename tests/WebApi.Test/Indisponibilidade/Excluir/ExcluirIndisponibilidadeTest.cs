using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Integracao.Test.Indisponibilidade.Excluir;

[TestFixture]
public class ExcluirIndisponibilidadeTest : FfkApiClassFixture
{
    private readonly string _baseUrlIndisponibilidade = "indisponibilidade";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroIndisponibilidades;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Indisponibilidade _indisponibilidadeNova;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova;

    public ExcluirIndisponibilidadeTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroIndisponibilidades = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroIndisponibilidades"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _indisponibilidadeNova = (FfkApi.Domain.Entities.Indisponibilidade)entidades["IndisponibilidadeNova"];
        _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova"];
    }

    private async Task<FfkApi.Domain.Entities.Indisponibilidade> CadastrarNovaIndisponibilidade(
        FfkApi.Domain.Entities.Usuario usuario,
        DateOnly dataInicial,
        DateOnly dataFinal)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = usuario.Email;
        request.DataInicial = dataInicial.ToString("dd/MM/yyyy");
        request.DataFinal = dataFinal.ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPost(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));

        var descricao = dadosDaResposta.RootElement.GetProperty("descricao").GetString();
        Assert.That(descricao, Is.EqualTo(request.Descricao));

        var dataInicialResposta = dadosDaResposta.RootElement.GetProperty("dataInicial").GetString();
        Assert.That(dataInicialResposta, Is.EqualTo(request.DataInicial));

        var dataFinalResposta = dadosDaResposta.RootElement.GetProperty("dataFinal").GetString();
        Assert.That(dataFinalResposta, Is.EqualTo(request.DataFinal));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("id").GetString()));
        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("nome").GetString()));

        var email = dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("email").GetString();
        Assert.That(email, Is.EqualTo(request.Usuario));

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("usuario").GetProperty("organizacao").GetString()));

        return new FfkApi.Domain.Entities.Indisponibilidade
        {
            Id = Guid.Parse(id!),
            Descricao = descricao!,
            DataInicial = dataInicial,
            DataFinal = dataFinal,
            IdUsuario = usuario.Id,
            Usuario = usuario
        };
    }

    [Test]
    public async Task Sucesso_Administrador_Excluindo_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(10)));

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

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroIndisponibilidades.Id);

        var indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioSemPerfilNemPermissao,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(11)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(11)));

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

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioSemPerfilNemPermissao,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(12)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(12)));

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

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioPermissaoCadastroIndisponibilidades,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(13)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(13)));

        var response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

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

        var indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(14)));

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlIndisponibilidade}/{indisponibilidade.Id}", token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroIndisponibilidades.Id);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlIndisponibilidade}/{_indisponibilidadeNova.Id}", cancellationToken, "tokenInvalid");

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

        var response = await HttpHelper.DoDelete($"{_baseUrlIndisponibilidade}/{_indisponibilidadeNova.Id}", cancellationToken);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlIndisponibilidade}/{_indisponibilidadeNova.Id}", cancellationToken, token);

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

        var response = await HttpHelper.DoDelete($"{_baseUrlIndisponibilidade}/{_indisponibilidadeNova.Id}", cancellationToken, token);

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{_indisponibilidadeNova.Id}", token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var response = await HttpHelper.DoDelete(url: $"{_baseUrlIndisponibilidade}/{_indisponibilidadeNova.Id}", token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
