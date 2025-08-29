using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using System.Net;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.InlineData;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Usuario.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2ECadastrarUsuarioTest : E2EClassFixture
{
    private readonly string _baseUrlUsuario = "usuario";

    [Test]
    public async Task Sucesso_Administrador_Com_Todos_Campos()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = _usuarioAdministrador.Organizacao.Nome;
        request.PerfisAcesso = [_perfilAcessoGerente.Nome];
        request.Permissoes = [_permissaoCadastroEquipes.Nome];

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var email = dadosDaResposta.RootElement.GetProperty("email").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(email));
        Assert.That(email, Is.EqualTo(request.Email));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));
        Assert.That(cpf, Is.EqualTo(request.Cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));
        Assert.That(telefone, Is.EqualTo(request.Telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo("Inativo"));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_usuarioAdministrador.Organizacao.Nome));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);
        Assert.That(perfisAcesso.ToListString(), Is.EquivalentTo(request.PerfisAcesso));

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);
        Assert.That(permissoes.ToListString(), Is.EquivalentTo(request.Permissoes));
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public async Task Sucesso_Administrador_Sem_Organizacao_Nem_Perfis_Nem_Permissoes(List<string>? listaVazia)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = listaVazia == null ? null : string.Empty;
        request.PerfisAcesso = listaVazia;
        request.Permissoes = listaVazia;

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var email = dadosDaResposta.RootElement.GetProperty("email").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(email));
        Assert.That(email, Is.EqualTo(request.Email));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));
        Assert.That(cpf, Is.EqualTo(request.Cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));
        Assert.That(telefone, Is.EqualTo(request.Telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo("Inativo"));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_usuarioAdministrador.Organizacao.Nome));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);
        Assert.That(perfisAcesso, Is.Empty);

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);
        Assert.That(permissoes, Is.Empty);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao_Com_Todos_Campos()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroUsuarios.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = _usuarioPermissaoCadastroUsuarios.Organizacao.Nome;
        request.PerfisAcesso = [_perfilAcessoGerente.Nome];
        request.Permissoes = [_permissaoCadastroEquipes.Nome];

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var email = dadosDaResposta.RootElement.GetProperty("email").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(email));
        Assert.That(email, Is.EqualTo(request.Email));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));
        Assert.That(cpf, Is.EqualTo(request.Cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));
        Assert.That(telefone, Is.EqualTo(request.Telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo("Inativo"));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_usuarioPermissaoCadastroUsuarios.Organizacao.Nome));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);
        Assert.That(perfisAcesso.ToListString(), Is.EquivalentTo(request.PerfisAcesso));

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);
        Assert.That(permissoes.ToListString(), Is.EquivalentTo(request.Permissoes));
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public async Task Sucesso_Usuario_Com_Permissao_Sem_Organizacao_Nem_Perfis_Nem_Permissoes(List<string>? listaVazia)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroUsuarios.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = listaVazia == null ? null : string.Empty;
        request.PerfisAcesso = listaVazia;
        request.Permissoes = listaVazia;

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var email = dadosDaResposta.RootElement.GetProperty("email").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(email));
        Assert.That(email, Is.EqualTo(request.Email));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));
        Assert.That(cpf, Is.EqualTo(request.Cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));
        Assert.That(telefone, Is.EqualTo(request.Telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo("Inativo"));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_usuarioPermissaoCadastroUsuarios.Organizacao.Nome));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);
        Assert.That(perfisAcesso, Is.Empty);

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);
        Assert.That(permissoes, Is.Empty);
    }

    [Test]
    public async Task Sucesso_Cpf_Existe_Em_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var organizacaoNova = await CadastroHelper.CadastrarNovaOrganizacao();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Cpf = _usuarioSemPerfilNemPermissao.Cpf;
        request.Organizacao = organizacaoNova.Nome;
        request.PerfisAcesso = [];
        request.Permissoes = [];

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        Assert.That(!string.IsNullOrWhiteSpace(dadosDaResposta.RootElement.GetProperty("id").GetString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(request.Nome));

        var email = dadosDaResposta.RootElement.GetProperty("email").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(email));
        Assert.That(email, Is.EqualTo(request.Email));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));
        Assert.That(cpf, Is.EqualTo(request.Cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));
        Assert.That(telefone, Is.EqualTo(request.Telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo("Inativo"));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(request.Organizacao));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);
        Assert.That(perfisAcesso, Is.Empty);

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);
        Assert.That(permissoes, Is.Empty);
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;
        request.Permissoes = null;

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var errors = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Usuários");

        Assert.That(errors.Count(), Is.EqualTo(1));
        Assert.That(errors.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));

    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request);

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

        var request = RequestCadastrarUsuarioBuilder.Build();

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token: "TokenInvalido");

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

        var request = RequestCadastrarUsuarioBuilder.Build();

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token: token);

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

        var request = RequestCadastrarUsuarioBuilder.Build();

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var erros = dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();

        var mensagemEsperada = MessagesException.GetString("TOKEN_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
        Assert.That(dadosDaResposta.RootElement.GetProperty("tokenEstaExpirado").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Erro_Email_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;
        request.Permissoes = null;
        request.Email = _usuarioSemPerfilNemPermissao.Email;

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAIL_JA_EXISTE");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Cpf_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;
        request.Permissoes = null;
        request.Cpf = _usuarioSemPerfilNemPermissao.Cpf;

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("CPF_JA_EXISTE");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Nome_Vazio(string? nome)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;
        request.Permissoes = null;
        request.Nome = nome;

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("NOME_VAZIO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.PerfisAcesso = null;
        request.Permissoes = null;

        var response = await HttpHelper.DoPost(_baseUrlUsuario, cancellationToken, request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nenhum_Perfil_Acesso_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.Permissoes = null;

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERFIS_ACESSO_NAO_ENCONTRADOS").Replace("{lista}", request.PerfisAcesso!.ListaSepadadaPorVirgula());

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Algum_Perfil_Acesso_Nao_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.Permissoes = null;
        request.PerfisAcesso = [_perfilAcessoGerente.Nome, "PerfilInvalido"];

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERFIS_ACESSO_NAO_ENCONTRADOS").Replace("{lista}", "PerfilInvalido");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Perfil_Acesso_Vazio(string? perfilAcesso)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.Permissoes = null;
        request.PerfisAcesso = [_perfilAcessoGerente.Nome, perfilAcesso!];

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERFIL_ACESSO_VAZIO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nenhuma_Permissao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERMISSOES_NAO_ENCONTRADAS").Replace("{lista}", request.Permissoes!.ListaSepadadaPorVirgula());

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Alguma_Permissao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;
        request.Permissoes = [_permissaoCadastroEquipes.Nome, "PermissaoInvalida"];

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERMISSOES_NAO_ENCONTRADAS").Replace("{lista}", "PermissaoInvalida");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Permissao_Vazia(string? permissao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;
        request.Permissoes = [_permissaoCadastroEquipes.Nome, permissao!];

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERMISSAO_VAZIA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;
        request.Permissoes = null;

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token, addAppToken: false);

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

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;
        request.PerfisAcesso = null;
        request.Permissoes = null;

        var response = await HttpHelper.DoPost(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
