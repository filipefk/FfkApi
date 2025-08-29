using FfkApi.Domain.Entities;
using FfkApi.Infrastructure.DataConfigurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FfkApi.Infrastructure.DataAccess;

public class FfkApiDbContext : DbContext
{
    public FfkApiDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Anexo> Anexos { get; set; }
    public DbSet<AuditoriaSeguranca> AuditoriasSeguranca { get; set; }
    public DbSet<Checklist> Checklists { get; set; }
    // ChecklistGatilhoRespostaPossivel cadastrado pela entidade Checklist
    // ChecklistItem cadastrado pela entidade Checklist
    public DbSet<ChecklistPreenchido> ChecklistsPreenchidos { get; set; }
    // ChecklistPreenchidoItem cadastrado pela entidade ChecklistPreenchido
    // ChecklistRespostaPossivel cadastrado pela entidade Checklist
    public DbSet<Equipe> Equipes { get; set; }
    public DbSet<Feed> Feeds { get; set; }
    public DbSet<Fila> Filas { get; set; }
    public DbSet<FilaItem> FilaItens { get; set; }
    public DbSet<Indisponibilidade> Indisponibilidades { get; set; }
    public DbSet<MembroEquipe> MembrosEquipe { get; set; }
    public DbSet<Organizacao> Organizacoes { get; set; }
    public DbSet<PerfilAcesso> PerfisAcesso { get; set; }
    public DbSet<Permissao> Permissoes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<SistemaCliente> SistemasCliente { get; set; }
    public DbSet<TokenAtivacao> TokensAtivacao { get; set; }
    public DbSet<TokenNovaSenha> TokensNovaSenha { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        AplicaConfiguracoesParaTodasClassesHerdeirasDeEntityBaseConfiguration(modelBuilder);
    }

    private static void AplicaConfiguracoesParaTodasClassesHerdeirasDeEntityBaseConfiguration(ModelBuilder modelBuilder)
    {
        foreach (var tipo in ListaClassesHerdeirasDeEntityBaseConfiguration())
        {
            dynamic classeInstanciada = Activator.CreateInstance(tipo)!;
            modelBuilder.ApplyConfiguration(classeInstanciada);
        }
    }

    private static List<Type> ListaClassesHerdeirasDeEntityBaseConfiguration()
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType != null &&
                        t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(EntityBaseConfiguration<>))
            .ToList();
    }
}
