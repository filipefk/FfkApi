using FfkApi.Domain.Entities;
using FfkApi.Domain.Enums;
using FfkApi.Infrastructure.DataAccess;
using System.Diagnostics;
using TestUtil.Criptografia;
using TestUtil.Entities;

namespace FfkApi.Initialization.DataInitialization;

public static class DadosDeTeste
{
    public static void Cadastrar(FfkApiDbContext dbContext)
    {
        CadastrarDadosDeTeste(dbContext);
    }

    private static void CadastrarDadosDeTeste(FfkApiDbContext dbContext)
    {
        if (dbContext.Usuarios.Any())
            return;

        CadastrarUsuariosTeste(dbContext);
        CadastrarEquipesTeste(dbContext);
        CadastrarIndisponibilidadesTeste(dbContext);
        CadastrarPerfisAcessoTeste(dbContext);
        CadastrarFeedsTeste(dbContext);

        //var checklists = dbContext
        //    .Checklists
        //    .Include(c => c.ChecklistItens.OrderBy(ci => ci.Ordem))
        //        .ThenInclude(ci => ci.ChecklistRespostasPossiveis.OrderBy(cr => cr.Ordem))
        //    .Include(c => c.ChecklistItens.OrderBy(ci => ci.Ordem))
        //        .ThenInclude(ci => ci.DependeDeChecklistItem)
        //    .Include(c => c.ChecklistItens.OrderBy(ci => ci.Ordem))
        //        .ThenInclude(ci => ci.GatilhosDeResposta)
        //        .ThenInclude(cr => cr.RespostaGatilho)
        //    .ToList();

        //var responseChecklists = new MapperConfiguration(options => { options.AddProfile(new AutoMapping()); }).CreateMapper().Map<List<ResponseChecklist>>(checklists);

        //string jsonString = JsonSerializer.Serialize(responseChecklists, new JsonSerializerOptions
        //{
        //    WriteIndented = true,
        //    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //});

        //var checklistsPreenchidos = dbContext
        //    .ChecklistsPreenchidos
        //    .Include(c => c.ChecklistPreenchidoItens.OrderBy(ci => ci.OrdemItem))
        //    .ToList();

        //var responseChecklistsPreenchidos = new MapperConfiguration(options => { options.AddProfile(new AutoMapping()); }).CreateMapper().Map<List<ResponseChecklistPreenchido>>(checklistsPreenchidos);

        //string jsonString = JsonSerializer.Serialize(responseChecklistsPreenchidos, new JsonSerializerOptions
        //{
        //    WriteIndented = true,
        //    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //});

        Debug.WriteLine($"Coloque o Breackpoint aqui");

    }

    private static void CadastrarUsuariosTeste(FfkApiDbContext dbContext)
    {
        if (!dbContext.Usuarios.Any())
        {
            var usuarios = new List<Usuario>();
            var senha = EncriptadorSenhaBuilder.Build().Encriptar("123456");

            for (int i = 0; i < 40; i++)
            {
                var usuario = UsuarioBuilder.Build();
                usuario.Senha = senha;
                usuarios.Add(usuario);
            }

            dbContext.Usuarios.AddRange(usuarios);
            dbContext.SaveChanges();
        }
    }

    private static void CadastrarEquipesTeste(FfkApiDbContext dbContext)
    {
        if (!dbContext.Equipes.Any())
        {
            var equipes = new List<Equipe>
            {
                new()
                {
                    Nome = "Equipe Alpha",
                    Descricao = "Equipe que atende primeiro",
                    Membros =
                    [
                        new() { IdUsuario = dbContext.Usuarios.First().Id, Lider = true },
                        new() { IdUsuario = dbContext.Usuarios.Skip(1).First().Id, Lider = false },
                        new() { IdUsuario = dbContext.Usuarios.Skip(2).First().Id, Lider = false }
                    ],
                },
                new()
                {
                    Nome = "Equipe Betha",
                    Descricao = "Equipe que faz certo",
                    Membros =
                    [
                        new() { IdUsuario = dbContext.Usuarios.Skip(3).First().Id, Lider = true },
                        new() { IdUsuario = dbContext.Usuarios.Skip(4).First().Id, Lider = false },
                        new() { IdUsuario = dbContext.Usuarios.Skip(5).First().Id, Lider = false }
                    ],
                },
                new()
                {
                    Nome = "Equipe Omega",
                    Descricao = "Equipe que manda bem",
                    Membros =
                    [
                        new() { IdUsuario = dbContext.Usuarios.Skip(6).First().Id, Lider = true },
                        new() { IdUsuario = dbContext.Usuarios.Skip(7).First().Id, Lider = false },
                        new() { IdUsuario = dbContext.Usuarios.Skip(8).First().Id, Lider = false }
                    ],
                }
            };

            dbContext.Equipes.AddRange(equipes);
            dbContext.SaveChanges();
        }
    }

    private static void CadastrarIndisponibilidadesTeste(FfkApiDbContext dbContext)
    {
        if (!dbContext.Indisponibilidades.Any())
        {
            var indisponibilidades = new List<Indisponibilidade>
            {
                new() { Descricao = "Licença médica", DataInicial = new DateOnly(2023, 1, 1), DataFinal = new DateOnly(2023, 1, 10), IdUsuario = dbContext.Usuarios.First().Id },
                new() { Descricao = "Férias", DataInicial = new DateOnly(2023, 2, 1), DataFinal = new DateOnly(2023, 2, 10), IdUsuario = dbContext.Usuarios.Skip(1).First().Id },
                new() { Descricao = "Folga", DataInicial = new DateOnly(2023, 3, 1), DataFinal = new DateOnly(2023, 3, 10), IdUsuario = dbContext.Usuarios.Skip(2).First().Id },
                new() { Descricao = "Férias", DataInicial = new DateOnly(2023, 4, 1), DataFinal = new DateOnly(2023, 4, 10), IdUsuario = dbContext.Usuarios.Skip(3).First().Id },
                new() { Descricao = "Filho doente", DataInicial = new DateOnly(2023, 5, 1), DataFinal = new DateOnly(2023, 5, 10), IdUsuario = dbContext.Usuarios.Skip(4).First().Id },
                new() { Descricao = "Férias", DataInicial = new DateOnly(2023, 6, 1), DataFinal = new DateOnly(2023, 6, 10), IdUsuario = dbContext.Usuarios.Skip(5).First().Id },
                new() { Descricao = "Folga", DataInicial = new DateOnly(2023, 7, 1), DataFinal = new DateOnly(2023, 7, 10), IdUsuario = dbContext.Usuarios.Skip(6).First().Id },
                new() { Descricao = "Licença médica", DataInicial = new DateOnly(2023, 8, 1), DataFinal = new DateOnly(2023, 8, 10), IdUsuario = dbContext.Usuarios.Skip(7).First().Id },
                new() { Descricao = "Viajem de negócio", DataInicial = new DateOnly(2023, 9, 1), DataFinal = new DateOnly(2023, 9, 10), IdUsuario = dbContext.Usuarios.Skip(8).First().Id },
            };

            dbContext.Indisponibilidades.AddRange(indisponibilidades);
            dbContext.SaveChanges();
        }
    }

    private static void CadastrarPerfisAcessoTeste(FfkApiDbContext dbContext)
    {
        if (!dbContext.PerfisAcesso.Any())
        {
            var perfisAcesso = new List<PerfilAcesso>
            {
                new() { Nome = "Líder de Equipe", Descricao = "Perfil com permissões de líder", Permissoes = dbContext.Permissoes.Take(10).ToList() },
                new() { Nome = "Visitante", Descricao = "Perfil com permissões de visualização", Permissoes = dbContext.Permissoes.Take(3).ToList() }
            };

            dbContext.PerfisAcesso.AddRange(perfisAcesso);
            dbContext.SaveChanges();
        }
    }

    private static void CadastrarFeedsTeste(FfkApiDbContext dbContext)
    {
        if (!dbContext.Feeds.Any())
        {
            var feeds = new List<Feed>
            {
                new() { Nome = "Comunicado: Suspensão de Sistema", Descricao = "Sistema de prenotação ficará indisponível para manutenção no dia 30/05, das 00h às 06h.", PalavrasChave = "sistema, manutenção, prenotação", Anexos = dbContext.Anexos.Skip(6).Take(2).ToList(), VisibilidadeUsuarios = dbContext.Usuarios.Skip(6).Take(2).ToList(), VisibilidadeEquipes = dbContext.Equipes.Skip(3).Take(1).ToList(), ExpiraEm = new DateOnly(2023, 5, 31), Status = StatusFeed.Rascunho },
            };

            dbContext.Feeds.AddRange(feeds);
            dbContext.SaveChanges();
        }
    }
}