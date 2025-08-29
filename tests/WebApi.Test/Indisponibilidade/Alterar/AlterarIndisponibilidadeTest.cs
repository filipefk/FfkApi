using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Integracao.Test.Indisponibilidade.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarIndisponibilidadeTest : FfkApiClassFixture
{
    private readonly string _baseUrlIndisponibilidade = "indisponibilidade";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroIndisponibilidades;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Indisponibilidade _indisponibilidadeNova;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova;
    private readonly FfkApi.Domain.Entities.Indisponibilidade _indisponibilidadeNovaOrganizacaoNova;

    public AlterarIndisponibilidadeTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroIndisponibilidades = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroIndisponibilidades"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _indisponibilidadeNova = (FfkApi.Domain.Entities.Indisponibilidade)entidades["IndisponibilidadeNova"];
        _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova"];
        _indisponibilidadeNovaOrganizacaoNova = (FfkApi.Domain.Entities.Indisponibilidade)entidades["IndisponibilidadeNovaOrganizacaoNova"];
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
    public async Task Sucesso_Administrador_Alterando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarIndisponibilidadeBuilder.Build(_indisponibilidadeNovaOrganizacaoNova);

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Alterando_Para_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroIndisponibilidades.Id);

        var indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioSemPerfilNemPermissao,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(6)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(6)));

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = indisponibilidade.Id.ToString();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(6).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(6).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Alterando_Pra_Si_Mesmo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioSemPerfilNemPermissao,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(7)));

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = indisponibilidade.Id.ToString();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Alterando_Para_Outro_Usuario()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = _indisponibilidadeNova.Id.ToString();
        request.Usuario = _indisponibilidadeNova.Usuario.Email;
        request.DataInicial = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Indisponibilidades");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Alterando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroIndisponibilidades.Id);

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = _indisponibilidadeNovaOrganizacaoNova.Id.ToString();
        request.Usuario = _usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova.Email;

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada1 = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");
        var mensagemEsperada2 = MessagesException.GetString("INDISPONIBILIDADE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(2));
        Assert.That(erros.Any(e => e.GetString() == mensagemEsperada1), Is.True);
        Assert.That(erros.Any(e => e.GetString() == mensagemEsperada2), Is.True);
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = _indisponibilidadeNova.Id.ToString();

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Indisponibilidade_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("INDISPONIBILIDADE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Existe_Indisponibilidade_Para_Usuario_No_Periodo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioSemPerfilNemPermissao,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(8)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(8)));

        indisponibilidade = await CadastrarNovaIndisponibilidade(
            usuario: _usuarioSemPerfilNemPermissao,
            dataInicial: DateOnly.FromDateTime(DateTime.Now.AddDays(9)),
            dataFinal: DateOnly.FromDateTime(DateTime.Now.AddDays(9)));

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = indisponibilidade.Id.ToString();
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("JA_EXISTE_INDISPONIBILIDADE_NO_PERIODO");

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

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = id;
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

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

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = id;
        request.Usuario = _usuarioSemPerfilNemPermissao.Email;
        request.DataInicial = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy");
        request.DataFinal = DateTime.Now.AddDays(5).ToString("dd/MM/yyyy");

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ID_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("      ")]
    public async Task Erro_Data_Final_Vazia(string? dataFinal)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = _indisponibilidadeNova.Id.ToString();
        request.Usuario = _indisponibilidadeNova.Usuario.Email;
        request.DataInicial = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
        request.DataFinal = dataFinal;

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DATA_FINAL_VAZIA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, cancellationToken: cancellationToken, request: request);

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

        var request = RequestAlterarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, cancellationToken: cancellationToken, request: request, token: "TokenInvalido");

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

        var request = RequestAlterarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestAlterarIndisponibilidadeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = _indisponibilidadeNova.Id.ToString();

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Id = _indisponibilidadeNova.Id.ToString();

        var response = await HttpHelper.DoPut(url: _baseUrlIndisponibilidade, request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
