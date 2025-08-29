using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Equipe.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EAlterarEquipeTest : E2EClassFixture
{
    private readonly string _baseUrlEquipe = "equipe";

    [Test]
    public async Task Sucesso_Administrador_Alterando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [],
            organizacao: organizacaoNova);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Nome = $"{request.Nome} e tal e coisa";

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Administrador_Alterando_A_Organizacao_De_Uma_Equipe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [],
            organizacao: organizacaoNova);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Id = equipe.Id.ToString();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.Membros = [];

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Alterando_Equipe_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroEquipes = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Equipes"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroEquipes.Id);

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao],
            organizacao: usuarioPermissaoCadastroEquipes.Organizacao);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Id = equipe.Id.ToString();
        request.Nome = $"{request.Nome} e tal e coisa";
        if (request.Membros != null && request.Membros.Count > 0)
            request.Membros.RemoveAt(0);

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Sem_Informar_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroEquipes = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Equipes"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroEquipes.Id);

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao],
            organizacao: usuarioPermissaoCadastroEquipes.Organizacao);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Id = equipe.Id.ToString();
        request.Nome = $"{request.Nome} e tal e coisa";
        request.Organizacao = null;

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Erro_Usuario_Com_Permissao_Alterando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioPermissaoCadastroEquipes = await CadastroHelper.CadastrarNovoUsuario(permissoes: ["Cadastro de Equipes"], ativar: true);

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(usuarioPermissaoCadastroEquipes.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [],
            organizacao: organizacaoNova);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Nome = $"{request.Nome} e tal e coisa";
        request.Organizacao = null;

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EQUIPE_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Administrador_Alterando_A_Organizacao_De_Uma_Equipe_Que_Tem_Membros()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao],
            organizacao: _usuarioAdministrador.Organizacao);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Organizacao = organizacaoNova.Nome;

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        Assert.That(erros.Count(), Is.EqualTo(2));
        var listaErros = erros.Select(e => e.GetString());
        Assert.That(listaErros, Contains.Item(MessagesException.GetString("IMPOSSIVEL_TROCAR_ORGANIZACAO_EQUIPE_QUANDO_TEM_MEMBROS")));
        var emails = request.Membros!.Select(m => m.Email).ToList();
        Assert.That(listaErros, Contains.Item(MessagesException.GetString("EMAILS_DE_USUARIOS_NAO_ENCONTRADOS").Replace("{lista}", emails!.ListaSepadadaPorVirgula())));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestAlterarEquipeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Equipes");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao]);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Id = equipe.Id.ToString();
        request.Membros = [];
        request.Organizacao = "Organizacao Inexistente";

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Emails_Usuarios_Nao_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao]);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Id = equipe.Id.ToString();
        var emailInvalido = "emailinexistente@provedor.com";
        request.Membros![0].Email = emailInvalido;

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAILS_DE_USUARIOS_NAO_ENCONTRADOS").Replace("{lista}", emailInvalido);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nome_De_Equipe_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var equipe1 = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao]);

        var equipe2 = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao]);

        var request = RequestAlterarEquipeBuilder.Build(equipe2);
        request.Id = equipe2.Id.ToString();
        request.Nome = equipe1.Nome;

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("NOME_DE_EQUIPE_JA_EXISTE_NA_ORGANIZACAO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("      ")]
    public async Task Erro_Descricao_Vazia(string? descricao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var equipe = await CadastroHelper.CadastrarNovaEquipe(
            usuariosEquipe: [_usuarioAdministrador, _usuarioSemPerfilNemPermissao]);

        var request = RequestAlterarEquipeBuilder.Build(equipe);
        request.Id = equipe.Id.ToString();
        request.Descricao = descricao;

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("DESCRICAO_VAZIA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, cancellationToken: cancellationToken, request: request);

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

        var request = RequestAlterarEquipeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, cancellationToken: cancellationToken, request: request, token: "TokenInvalido");

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

        var request = RequestAlterarEquipeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestAlterarEquipeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var erros = dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();

        var mensagemEsperada = MessagesException.GetString("TOKEN_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
        Assert.That(dadosDaResposta.RootElement.GetProperty("tokenEstaExpirado").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Erro_Equipe_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarEquipeBuilder.Build();
        request.Organizacao = null;
        request.Membros = [];

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EQUIPE_NAO_ENCONTRADA");

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

        var request = RequestAlterarEquipeBuilder.Build();
        request.Id = id;
        request.Organizacao = null;
        request.Membros = [];

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

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

        var request = RequestAlterarEquipeBuilder.Build();
        request.Id = id;
        request.Organizacao = null;
        request.Membros = [];

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken);

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

        var request = RequestAlterarEquipeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken, addAppToken: false);

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

        var request = RequestAlterarEquipeBuilder.Build();

        var response = await HttpHelper.DoPut(url: _baseUrlEquipe, request: request, token: token, cancellationToken: cancellationToken, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
