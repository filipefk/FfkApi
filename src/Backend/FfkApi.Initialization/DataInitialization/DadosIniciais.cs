using FfkApi.Domain.Entities;
using FfkApi.Infrastructure.DataAccess;
using TestUtil.Criptografia;

namespace FfkApi.Initialization.DataInitialization;

public static class DadosIniciais
{
    public static Usuario UsuarioAdministrador = null!;

    public static void Cadastrar(FfkApiDbContext dbContext)
    {
        CadastrarOrganizacaoInicial(dbContext);
        CadastrarPermissoesIniciais(dbContext);
        CadastrarPerfisAcessoIniciais(dbContext);
        CadastrarUsuariosIniciais(dbContext);
    }

    private static void CadastrarOrganizacaoInicial(FfkApiDbContext dbContext)
    {
        if (!dbContext.Organizacoes.Any())
        {
            var organizacao = new Organizacao()
            {
                Nome = "FfkApi",
                Descricao = "Organização responsável pelo sistema",
                RemetenteEmail = "ffkapi@test-2p0347z8er3lzdrn.mlsender.net",
                RemetenteNome = "Sistema FfkApi",
                ModeloEmailAtivacao = "3vz9dlejrw14kj50",
                ModeloEmailNovaSenha = "yzkq340kpw2gd796",
            };
            dbContext.Organizacoes.Add(organizacao);
            dbContext.SaveChanges();
        }
    }

    private static void CadastrarUsuariosIniciais(FfkApiDbContext dbContext)
    {
        if (!dbContext.Usuarios.Any())
        {
            var organizacao = dbContext.Organizacoes.FirstOrDefault();
            var senhaNaoEncriptada = "Senha.InicialFfkApiAdmin1";
            var senhaEncriptada = EncriptadorSenhaBuilder.Build().Encriptar(senhaNaoEncriptada);
            var perfilAdministrador = dbContext.PerfisAcesso.FirstOrDefault(p => p.Nome.Equals("Administrador"));
            var usuarioAdministrador = new Usuario()
            {
                Nome = "Admin",
                Email = "admin@ffkapi.com",
                Cpf = "16114573005",
                Telefone = "+5523989509771",
                Senha = senhaEncriptada,
                Status = Domain.Enums.StatusUsuario.Ativo,
                PerfisAcesso = [perfilAdministrador!],
                Organizacao = organizacao!,
            };
            dbContext.Usuarios.Add(usuarioAdministrador);
            dbContext.SaveChanges();
            UsuarioAdministrador = new Usuario()
            {
                Id = usuarioAdministrador.Id,
                Nome = usuarioAdministrador.Nome,
                Email = usuarioAdministrador.Email,
                Cpf = usuarioAdministrador.Cpf,
                Telefone = usuarioAdministrador.Telefone,
                Senha = senhaNaoEncriptada,
                Status = usuarioAdministrador.Status,
                PerfisAcesso = usuarioAdministrador.PerfisAcesso,
                IdOrganizacao = usuarioAdministrador.IdOrganizacao,
                Organizacao = usuarioAdministrador.Organizacao,
            };

            senhaNaoEncriptada = "Senha.valida1";
            senhaEncriptada = EncriptadorSenhaBuilder.Build().Encriptar(senhaNaoEncriptada);
            var usuario2 = new Usuario()
            {
                Nome = "SemPerfilNemPermissao",
                Email = "SemPerfilNemPermissao@provedor.com",
                Cpf = "52522176021",
                Telefone = "+5585992354800",
                Senha = senhaEncriptada,
                Status = Domain.Enums.StatusUsuario.Ativo,
                Organizacao = organizacao!,
            };
            dbContext.Usuarios.Add(usuario2);
            dbContext.SaveChanges();

            var permissao = dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Usuários"));
            var usuario3 = new Usuario()
            {
                Nome = "PermissaoCadastroUsuarios",
                Email = "PermissaoCadastroUsuarios@provedor.com",
                Cpf = "83593654822",
                Telefone = "+5589998187583",
                Senha = senhaEncriptada,
                Status = Domain.Enums.StatusUsuario.Ativo,
                Permissoes = [permissao!],
                Organizacao = organizacao!,
            };
            dbContext.Usuarios.Add(usuario3);
            dbContext.SaveChanges();
        }
    }

    private static void CadastrarPermissoesIniciais(FfkApiDbContext dbContext)
    {
        if (!dbContext.Permissoes.Any())
        {
            var permissoes = new List<Permissao>
            {
                new() { Nome = "Cadastro de Usuários", Descricao = "Permite cadastrar novos usuários e alterar usuários já existentes." },
                new() { Nome = "Cadastro de Organizações", Descricao = "Permite cadastrar novas organizações e alterar organizações já existentes." },
                new() { Nome = "Cadastro de Indisponibilidades", Descricao = "Permite cadastrar novas indisponibilidades de usuários e alterar indisponibilidade de usuários já existentes." },
                new() { Nome = "Cadastro de Feeds", Descricao = "Permite cadastrar novos feeds e alterar feeds já existentes." },
                new() { Nome = "Cadastro de Equipes", Descricao = "Permite cadastrar novas equipes e alterar equipes já existentes." },
                new() { Nome = "Cadastro de perfil de permissão de usuários", Descricao = "Permite cadastrar novos perfil e alterar perfil já existentes." },
                new() { Nome = "Cadastro de Formulários/Check Lists", Descricao = "Permite cadastrar novos formulários e check lists e alterar formulários e check lists já existentes." },
            };

            dbContext.Permissoes.AddRange(permissoes);
            dbContext.SaveChanges();
        }
    }

    private static void CadastrarPerfisAcessoIniciais(FfkApiDbContext dbContext)
    {
        if (!dbContext.PerfisAcesso.Any())
        {
            var perfisAcesso = new List<PerfilAcesso>
            {
                new()
                {
                    Nome = "Administrador",
                    Descricao = "Perfil que tem todas as permissões do sistema."
                },
                new()
                {
                    Nome = "Cadastrador de usuários e equipes",
                    Descricao = "Permite cadastrar e alterar os dados de usuários do sistema e de equipes.",
                    Permissoes = new List<Permissao>
                    {
                        dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Usuários"))!,
                        dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Equipes"))!,
                    }
                },
                new()
                {
                    Nome = "Gerente",
                    Descricao = "Permite fazer a gestão das equipes.",
                    Permissoes = new List<Permissao>
                    {
                        dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Usuários"))!,
                        dbContext.Permissoes.FirstOrDefault(p => p.Nome.Equals("Cadastro de Equipes"))!,
                    }
                },
            };

            dbContext.PerfisAcesso.AddRange(perfisAcesso);
            dbContext.SaveChanges();
        }
    }
}
