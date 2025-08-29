using Aceitacao.Test.Helpers;
using FfkApi.Domain.Services.Email;
using FfkApi.Domain.Services.Mensageria;
using FfkApi.Infrastructure.Services.Email;
using FfkApi.Infrastructure.Services.Mensageria;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aceitacao.Test;

public class E2EClassFixture
{
    private const string _appToken = "testes-2blHt60aerveQI2UaASZssPntfaB8alE6uJRnQdvbkk";
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _factory;
    protected FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    protected FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    protected FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroUsuarios;
    protected FfkApi.Domain.Entities.PerfilAcesso _perfilAcessoGerente;
    protected FfkApi.Domain.Entities.Permissao _permissaoCadastroEquipes;
    protected HttpHelper HttpHelper { get; }
    protected CadastroHelper CadastroHelper { get; }

    public E2EClassFixture()
    {
        Environment.SetEnvironmentVariable("RODANDO_TESTE_ACEITACAO", "true");

        _usuarioAdministrador = new FfkApi.Domain.Entities.Usuario()
        {
            Nome = "Admin",
            Email = "admin@ffkapi.com",
            Senha = "Senha.InicialFfkApiAdmin1",
        };

        _usuarioSemPerfilNemPermissao = new FfkApi.Domain.Entities.Usuario()
        {
            Nome = "SemPerfilNemPermissao",
            Email = "SemPerfilNemPermissao@provedor.com",
            Senha = "Senha.valida1",
        };

        _usuarioPermissaoCadastroUsuarios = new FfkApi.Domain.Entities.Usuario()
        {
            Nome = "PermissaoCadastroUsuarios",
            Email = "PermissaoCadastroUsuarios@provedor.com",
            Senha = "Senha.valida1",
        };

        _perfilAcessoGerente = new FfkApi.Domain.Entities.PerfilAcesso()
        {
            Nome = "Gerente",
            Descricao = "Permite fazer a gestão das equipes.",
        };

        _permissaoCadastroEquipes = new FfkApi.Domain.Entities.Permissao()
        {
            Nome = "Cadastro de Equipes",
            Descricao = "Permite cadastrar novas equipes e alterar equipes já existentes.",
        };

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IEnviarEmailService>();
                    services.AddScoped<IEnviarEmailService, EnviarEmailFakeService>();
                    services.RemoveAll<IPublicarMensagemService>();
                    services.AddScoped<IPublicarMensagemService, PublicarMensagemFakeService>();
                });
            });

        _httpClient = _factory.CreateClient();

        HttpHelper = new HttpHelper(_httpClient, _appToken);
        CadastroHelper = new CadastroHelper(HttpHelper, _usuarioAdministrador, _usuarioSemPerfilNemPermissao);

        CadastroHelper.PegarDadosUsuario(_usuarioAdministrador).GetAwaiter().GetResult();
        CadastroHelper.PegarDadosUsuario(_usuarioSemPerfilNemPermissao).GetAwaiter().GetResult();
        CadastroHelper.PegarDadosUsuario(_usuarioPermissaoCadastroUsuarios).GetAwaiter().GetResult();
    }
}
