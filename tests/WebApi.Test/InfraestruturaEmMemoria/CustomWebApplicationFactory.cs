using FfkApi.Domain.Services.Arquivos;
using FfkApi.Domain.Services.Email;
using FfkApi.Infrastructure.DataAccess;
using FfkApi.Infrastructure.Services.Email;
using FfkApi.Initialization.DataInitialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TestUtil.Criptografia;
using TestUtil.Entities;
using TestUtil.Services;

namespace Integracao.Test.InfraestruturaEmMemoria;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected Dictionary<string, object> _entidadesCriadas = [];

    public Dictionary<string, object> EntidadesCriadas() => _entidadesCriadas;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<FfkApiDbContext>)
                );

                if (descriptor is not null)
                    services.Remove(descriptor);

                var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<FfkApiDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(provider);
                });

                services.RemoveAll<IArmazenadorDeArquivoService>();
                services.AddSingleton<IArmazenadorDeArquivoService, ArmazenadorDeArquivoEmMemoriaService>();

                services.RemoveAll<IEnviarEmailService>();
                services.AddScoped<IEnviarEmailService, EnviarEmailFakeService>();

                using var scope = services.BuildServiceProvider().CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<FfkApiDbContext>();

                StartDatabase(dbContext);

            });
    }

    private void StartDatabase(FfkApiDbContext dbContext)
    {
        DadosIniciais.Cadastrar(dbContext);

        var organizacaoFfkApi = dbContext.Organizacoes.FirstOrDefault(organizacao => organizacao.Nome.Equals("FfkApi"));
        _entidadesCriadas["OrganizacaoFfkApi"] = organizacaoFfkApi!;

        var perfilAcessoAdministrador = dbContext.PerfisAcesso.FirstOrDefault(p => p.Nome.Equals("Administrador"))!;
        _entidadesCriadas["PerfilAcessoAdministrador"] = perfilAcessoAdministrador!;

        var perfilAcessoGerente = dbContext.PerfisAcesso.FirstOrDefault(p => p.Nome.Equals("Gerente"))!;
        _entidadesCriadas["PerfilAcessoGerente"] = perfilAcessoGerente!;

        var permissaoCadastroUsuarios = dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Usuários"))!;
        _entidadesCriadas["PermissaoCadastroUsuarios"] = permissaoCadastroUsuarios!;

        var permissaoCadastroEquipes = dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Equipes"))!;
        _entidadesCriadas["PermissaoCadastroEquipes"] = permissaoCadastroEquipes!;

        var permissaoCadastroOrganizacoes = dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Organizações"))!;
        _entidadesCriadas["PermissaoCadastroOrganizacoes"] = permissaoCadastroOrganizacoes!;

        var permissaoCadastroFeeds = dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Feeds"))!;
        _entidadesCriadas["PermissaoCadastroFeeds"] = permissaoCadastroFeeds!;

        var permissaoCadastroIndisponibilidades = dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Indisponibilidades"))!;
        _entidadesCriadas["PermissaoCadastroIndisponibilidades"] = permissaoCadastroIndisponibilidades!;

        // Fim do bloco de permissoes <== Para uso do gerador de código

        _entidadesCriadas["UsuarioAdministrador"] = DadosIniciais.UsuarioAdministrador;

        var usuarioPermissaoCadastroUsuarios = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioPermissaoCadastroUsuarios.Permissoes = [permissaoCadastroUsuarios];
        usuarioPermissaoCadastroUsuarios.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioPermissaoCadastroUsuarios, "UsuarioPermissaoCadastroUsuarios");

        var usuarioPermissaoCadastroOrganizacoes = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioPermissaoCadastroOrganizacoes.Permissoes = [permissaoCadastroOrganizacoes];
        usuarioPermissaoCadastroOrganizacoes.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioPermissaoCadastroOrganizacoes, "UsuarioPermissaoCadastroOrganizacoes");

        var usuarioSemPerfilNemPermissao = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioSemPerfilNemPermissao.Permissoes = [];
        usuarioSemPerfilNemPermissao.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioSemPerfilNemPermissao, "UsuarioSemPerfilNemPermissao");

        var usuarioInativoComTokenAtivacaoValido = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioInativoComTokenAtivacaoValido.Status = FfkApi.Domain.Enums.StatusUsuario.Inativo;
        usuarioInativoComTokenAtivacaoValido.Permissoes = [];
        usuarioInativoComTokenAtivacaoValido.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioInativoComTokenAtivacaoValido, "UsuarioInativoComTokenAtivacaoValido");

        var usuarioInativoComTokenAtivacaoVencido = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioInativoComTokenAtivacaoVencido.Status = FfkApi.Domain.Enums.StatusUsuario.Inativo;
        usuarioInativoComTokenAtivacaoVencido.Permissoes = [];
        usuarioInativoComTokenAtivacaoVencido.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioInativoComTokenAtivacaoVencido, "UsuarioInativoComTokenAtivacaoVencido");

        var usuarioInativoSemTokenAtivacao = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioInativoSemTokenAtivacao.Status = FfkApi.Domain.Enums.StatusUsuario.Inativo;
        usuarioInativoSemTokenAtivacao.Permissoes = [];
        usuarioInativoSemTokenAtivacao.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioInativoSemTokenAtivacao, "UsuarioInativoSemTokenAtivacao");

        var usuarioAtivoComTokenNovaSenhaValido = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioAtivoComTokenNovaSenhaValido.Permissoes = [];
        usuarioAtivoComTokenNovaSenhaValido.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioAtivoComTokenNovaSenhaValido, "UsuarioAtivoComTokenNovaSenhaValido");

        var usuarioAtivoComTokenNovaSenhaVencido = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioAtivoComTokenNovaSenhaVencido.Permissoes = [];
        usuarioAtivoComTokenNovaSenhaVencido.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioAtivoComTokenNovaSenhaVencido, "UsuarioAtivoComTokenNovaSenhaVencido");

        var usuarioParaTrocarSenha = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioParaTrocarSenha.Permissoes = [];
        usuarioParaTrocarSenha.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioParaTrocarSenha, "UsuarioParaTrocarSenha");

        var usuarioAlterarStatus = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioAlterarStatus.Permissoes = [];
        usuarioAlterarStatus.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioAlterarStatus, "UsuarioAlterarStatus");

        var usuarioAlterarSeusDados = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioAlterarSeusDados.Permissoes = [];
        usuarioAlterarSeusDados.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioAlterarSeusDados, "UsuarioAlterarSeusDados");

        var usuarioTrocarOrganizacao = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioTrocarOrganizacao.Permissoes = [];
        usuarioTrocarOrganizacao.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioTrocarOrganizacao, "UsuarioTrocarOrganizacao");

        var usuarioRemoverPermissoes = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioRemoverPermissoes.Permissoes = [permissaoCadastroOrganizacoes];
        usuarioRemoverPermissoes.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioRemoverPermissoes, "UsuarioRemoverPermissoes");

        var usuarioGanharPermissoes1 = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioGanharPermissoes1.Permissoes = [];
        usuarioGanharPermissoes1.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioGanharPermissoes1, "UsuarioGanharPermissoes1");

        var usuarioGanharPermissoes2 = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioGanharPermissoes2.Permissoes = [];
        usuarioGanharPermissoes2.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioGanharPermissoes2, "UsuarioGanharPermissoes2");

        var usuarioExcluir1 = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioExcluir1.Permissoes = [];
        usuarioExcluir1.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioExcluir1, "UsuarioExcluir1");

        var usuarioExcluir2 = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioExcluir2.Permissoes = [];
        usuarioExcluir2.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioExcluir2, "UsuarioExcluir2");

        var usuarioExcluir3 = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioExcluir3.Permissoes = [];
        usuarioExcluir3.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioExcluir3, "UsuarioExcluir3");

        var usuarioPermissaoCadastroFeeds = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioPermissaoCadastroFeeds.Permissoes = [permissaoCadastroFeeds];
        usuarioPermissaoCadastroFeeds.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioPermissaoCadastroFeeds, "UsuarioPermissaoCadastroFeeds");

        var usuarioPermissaoCadastroEquipes = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioPermissaoCadastroEquipes.Permissoes = [permissaoCadastroEquipes];
        usuarioPermissaoCadastroEquipes.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioPermissaoCadastroEquipes, "UsuarioPermissaoCadastroEquipes");

        var usuarioPermissaoCadastroIndisponibilidades = UsuarioBuilder.Build(organizacao: organizacaoFfkApi!);
        usuarioPermissaoCadastroIndisponibilidades.Permissoes = [permissaoCadastroIndisponibilidades];
        usuarioPermissaoCadastroIndisponibilidades.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioPermissaoCadastroIndisponibilidades, "UsuarioPermissaoCadastroIndisponibilidades");

        // Fim do bloco de usuários <== Para uso do gerador de código

        var tokenAtivacaoValido = TokenAtivacaoBuilder.Build(usuarioInativoComTokenAtivacaoValido);
        dbContext.TokensAtivacao.Add(tokenAtivacaoValido);
        dbContext.SaveChanges();
        _entidadesCriadas["TokenAtivacaoValido"] = tokenAtivacaoValido;

        var tokenAtivacaoVencido = TokenAtivacaoBuilder.Build(usuarioInativoComTokenAtivacaoVencido);
        tokenAtivacaoVencido.BaseExpiracaoUtc = tokenAtivacaoVencido.BaseExpiracaoUtc.AddYears(-1);
        dbContext.TokensAtivacao.Add(tokenAtivacaoVencido);
        dbContext.SaveChanges();
        _entidadesCriadas["TokenAtivacaoVencido"] = tokenAtivacaoVencido;

        var tokenNovaSenhaValido = TokenNovaSenhaBuilder.Build(usuarioAtivoComTokenNovaSenhaValido);
        dbContext.TokensNovaSenha.Add(tokenNovaSenhaValido);
        dbContext.SaveChanges();
        _entidadesCriadas["TokenNovaSenhaValido"] = tokenNovaSenhaValido;

        var tokenNovaSenhaVencido = TokenNovaSenhaBuilder.Build(usuarioAtivoComTokenNovaSenhaVencido);
        tokenNovaSenhaVencido.DataCriacaoUtc = tokenNovaSenhaVencido.DataCriacaoUtc.AddYears(-1);
        dbContext.TokensNovaSenha.Add(tokenNovaSenhaVencido);
        dbContext.SaveChanges();
        _entidadesCriadas["TokenNovaSenhaVencido"] = tokenNovaSenhaVencido;

        var organizacaoNova = OrganizacaoBuilder.Build("Nova");
        dbContext.Organizacoes.Add(organizacaoNova);
        dbContext.SaveChanges();
        _entidadesCriadas["OrganizacaoNova"] = organizacaoNova;

        var usuarioSemPerfilNemPermissaoOrganizacaoNova = UsuarioBuilder.Build(organizacao: organizacaoNova);
        usuarioSemPerfilNemPermissaoOrganizacaoNova.Permissoes = [];
        usuarioSemPerfilNemPermissaoOrganizacaoNova.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioSemPerfilNemPermissaoOrganizacaoNova, "UsuarioSemPerfilNemPermissaoOrganizacaoNova");

        var usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova = UsuarioBuilder.Build(organizacao: organizacaoNova);
        usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova.Permissoes = [permissaoCadastroIndisponibilidades];
        usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova.PerfisAcesso = [];
        SalvaUsuario(dbContext, usuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova, "UsuarioPermissaoCadastroIndisponibilidadesOrganizacaoNova");

        var sistemaClienteNovo = SistemaClienteBuilder.Build();
        var senhaNaoEncriptada = sistemaClienteNovo.Senha!;
        sistemaClienteNovo.Senha = EncriptadorSenhaBuilder.Build().Encriptar(senhaNaoEncriptada);
        dbContext.SistemasCliente.Add(sistemaClienteNovo);
        dbContext.SaveChanges();
        _entidadesCriadas["SistemaClienteNovo"] = CopiarSistemaCliente(sistemaClienteNovo, senhaNaoEncriptada);

        var anexoNovo = AnexoBuilder.Build();
        dbContext.Anexos.Add(anexoNovo);
        dbContext.SaveChanges();
        _entidadesCriadas["AnexoNovo"] = anexoNovo;

        var feedNovo = FeedBuilder.Build();
        dbContext.Feeds.Add(feedNovo);
        dbContext.SaveChanges();
        _entidadesCriadas["FeedNovo"] = feedNovo;

        var equipeNova = EquipeBuilder.Build(organizacao: organizacaoFfkApi);
        dbContext.Equipes.Add(equipeNova);
        dbContext.SaveChanges();
        _entidadesCriadas["EquipeNova"] = equipeNova;

        var indisponibilidadeNova = IndisponibilidadeBuilder.Build();
        indisponibilidadeNova.DataInicial = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        indisponibilidadeNova.DataFinal = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        indisponibilidadeNova.IdUsuario = DadosIniciais.UsuarioAdministrador.Id;
        indisponibilidadeNova.Usuario = default!;
        dbContext.Indisponibilidades.Add(indisponibilidadeNova);
        dbContext.SaveChanges();
        _entidadesCriadas["IndisponibilidadeNova"] = indisponibilidadeNova;

        var indisponibilidadeNovaOrganizacaoNova = IndisponibilidadeBuilder.Build();
        indisponibilidadeNovaOrganizacaoNova.DataInicial = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        indisponibilidadeNovaOrganizacaoNova.DataFinal = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        indisponibilidadeNovaOrganizacaoNova.IdUsuario = usuarioSemPerfilNemPermissaoOrganizacaoNova.Id;
        indisponibilidadeNovaOrganizacaoNova.Usuario = default!;
        dbContext.Indisponibilidades.Add(indisponibilidadeNovaOrganizacaoNova);
        dbContext.SaveChanges();
        _entidadesCriadas["IndisponibilidadeNovaOrganizacaoNova"] = indisponibilidadeNovaOrganizacaoNova;
    }

    private void SalvaUsuario(FfkApiDbContext dbContext, FfkApi.Domain.Entities.Usuario usuario, string nomeEntidade)
    {
        var senhaNaoEncriptada = usuario.Senha!;
        usuario.Senha = EncriptadorSenhaBuilder.Build().Encriptar(senhaNaoEncriptada);
        dbContext.Usuarios.Add(usuario);
        dbContext.SaveChanges();
        _entidadesCriadas[nomeEntidade] = CopiarUsuario(usuario, senhaNaoEncriptada);
    }

    private static FfkApi.Domain.Entities.Usuario CopiarUsuario(FfkApi.Domain.Entities.Usuario usuario, string? senhaNaoEncriptada = null)
    {
        return new FfkApi.Domain.Entities.Usuario
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Senha = senhaNaoEncriptada ?? usuario.Senha,
            Cpf = usuario.Cpf,
            Status = usuario.Status,
            Telefone = usuario.Telefone,
            IdOrganizacao = usuario.IdOrganizacao,
            Organizacao = usuario.Organizacao,
            PerfisAcesso = usuario.PerfisAcesso,
            Permissoes = usuario.Permissoes
        };
    }

    private static FfkApi.Domain.Entities.SistemaCliente CopiarSistemaCliente(FfkApi.Domain.Entities.SistemaCliente sistemaCliente, string? senhaNaoEncriptada = null)
    {
        return new FfkApi.Domain.Entities.SistemaCliente
        {
            Id = sistemaCliente.Id,
            AppId = sistemaCliente.AppId,
            Nome = sistemaCliente.Nome,
            Descricao = sistemaCliente.Descricao,
            Senha = senhaNaoEncriptada ?? sistemaCliente.Senha,
            Status = sistemaCliente.Status
        };
    }
}
