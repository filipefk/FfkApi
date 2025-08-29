using FfkApi.Domain.Entities;
using FfkApi.Domain.Services.Acesso;
using FfkApi.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FfkApi.Infrastructure.Services.Acesso;

public class AcessoService : IAcessoService
{
    private readonly FfkApiDbContext _dbContext;

    public AcessoService(FfkApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AplicarPerfisAoUsuarioAsync(Guid idUsuario, IList<Guid> perfilIds)
    {
        var usuario = await _dbContext
            .Usuarios
            .Include(u => u.PerfisAcesso)
            .Include(u => u.Permissoes)
            .FirstOrDefaultAsync(u => u.Id == idUsuario);

        if (usuario == null)
            throw new InvalidOperationException("Usuário não encontrado."); // TODO : Criar exceção específica

        var perfis = await _dbContext
            .PerfisAcesso
            .Include(p => p.Permissoes)
            .Where(p => perfilIds.Contains(p.Id))
            .ToListAsync();

        usuario.PerfisAcesso = perfis;

        var permissoesConsolidadas = perfis
            .SelectMany(p => p.Permissoes)
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .ToList();

        usuario.Permissoes = usuario.Permissoes
            .Union(permissoesConsolidadas)
            .ToList();
    }

    public async Task<bool> UsuarioTemPermissaoAsync(Guid idUsuario, string nomePermissao)
    {
        var usuario = await _dbContext.Set<Usuario>()
            .Include(u => u.PerfisAcesso)
            .Include(u => u.Permissoes)
            .FirstOrDefaultAsync(u => u.Id == idUsuario);

        if (usuario == null)
            return false;

        if (usuario.PerfisAcesso.Any(p => p.Nome.Equals("Administrador", StringComparison.OrdinalIgnoreCase)))
            return true;

        return usuario.Permissoes.Any(p => p.Nome.Equals(nomePermissao, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> UsuarioAdministradorAsync(Guid idUsuario)
    {
        var usuario = await _dbContext.Set<Usuario>()
            .Include(u => u.PerfisAcesso)
            .FirstOrDefaultAsync(u => u.Id == idUsuario);

        if (usuario == null)
            return false;

        if (usuario.PerfisAcesso.Any(p => p.Nome.Equals("Administrador", StringComparison.OrdinalIgnoreCase)))
            return true;

        return false;
    }


    public async Task AtualizarUsuariosDoPerfilAsync(Guid idPerfil, bool aplicarAlteracoes)
    {
        if (!aplicarAlteracoes)
            return;

        var perfil = await _dbContext
            .PerfisAcesso
            .Include(p => p.Permissoes)
            .FirstOrDefaultAsync(p => p.Id == idPerfil);

        if (perfil == null)
            throw new InvalidOperationException("Perfil não encontrado."); // TODO : Criar exceção específica

        var usuarios = await _dbContext
            .Usuarios
            .Include(u => u.PerfisAcesso)
            .Include(u => u.Permissoes)
            .Where(u => u.PerfisAcesso.Any(p => p.Id == idPerfil))
            .ToListAsync();

        foreach (var usuario in usuarios)
        {
            var permissoesDosPerfis = usuario.PerfisAcesso
                .SelectMany(p => p.Permissoes)
                .GroupBy(p => p.Id)
                .Select(g => g.First());

            var permissoesIndividuais = usuario.Permissoes
                .Where(p => !usuario.PerfisAcesso.SelectMany(perfil => perfil.Permissoes)
                .Any(pp => pp.Id == p.Id));

            usuario.Permissoes = permissoesIndividuais
                .Union(permissoesDosPerfis)
                .ToList();
        }

        //await _dbContext.SaveChangesAsync();
    }
}

