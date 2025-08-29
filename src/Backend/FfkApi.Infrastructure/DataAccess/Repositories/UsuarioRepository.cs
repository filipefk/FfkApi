using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly IGeradorTokenAtivacao _geradorTokenAtivacao;
    private readonly IGeradorTokenNovaSenha _geradorTokenNovaSenha;
    private readonly AsyncRetryPolicy _retryPolicy;

    public UsuarioRepository(
        FfkApiDbContext dbContext,
        IGeradorTokenAtivacao geradorTokenAtivacao,
        IGeradorTokenNovaSenha geradorTokenNovaSenha,
        AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _geradorTokenAtivacao = geradorTokenAtivacao;
        _geradorTokenNovaSenha = geradorTokenNovaSenha;
        _retryPolicy = retryPolicy;
    }

    public IQueryable<Usuario> AsQueryable()
    {
        return _dbContext
            .Usuarios
            .Include(usuario => usuario.PerfisAcesso)
            .Include(usuario => usuario.Permissoes)
            .Include(usuario => usuario.Organizacao)
            .AsQueryable();
    }

    public async Task<long> QuantidadeTotal(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task Adicionar(Usuario usuario, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.AddAsync(usuario, ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteUsuarioComEmail(string email, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.AnyAsync(usuario => usuario.Email == email, ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteUsuarioComCpf(string cpf, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.AnyAsync(usuario => usuario.Cpf == cpf, ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteUsuarioComCpf(string cpf, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.AnyAsync(usuario => usuario.Cpf == cpf && usuario.IdOrganizacao == idOrganizacao, ct),
            cancellationToken
        );
    }

    public async Task<Usuario?> PegarUsuarioAptoPorEmail(string email, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .Include(u => u.Organizacao)
                .FirstOrDefaultAsync(usuario => usuario.Email.Equals(email)
                    && (usuario.Status == Domain.Enums.StatusUsuario.Ativo
                        || usuario.Status == Domain.Enums.StatusUsuario.Ausente)
                    , ct),
            cancellationToken
        );
    }

    public async Task<Usuario?> PegarUsuarioAptoPorEmail(string email, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .Include(u => u.Organizacao)
                .FirstOrDefaultAsync(usuario => usuario.Email.Equals(email)
                    && usuario.IdOrganizacao == idOrganizacao
                    && (usuario.Status == Domain.Enums.StatusUsuario.Ativo
                        || usuario.Status == Domain.Enums.StatusUsuario.Ausente)
                    , ct),
            cancellationToken
        );
    }

    public async Task<IList<Usuario>> PegarUsuariosAptosPorEmails(IList<string> emails, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .Where(u => emails.Contains(u.Email)
                    && (u.Status == Domain.Enums.StatusUsuario.Ativo
                        || u.Status == Domain.Enums.StatusUsuario.Ausente))
                .ToListAsync(ct),
            cancellationToken
        );
    }

    public async Task<IList<Usuario>> PegarUsuariosAptosPorEmails(IList<string> emails, string nomeOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .Where(u => emails.Contains(u.Email)
                    && (u.Status == Domain.Enums.StatusUsuario.Ativo
                        || u.Status == Domain.Enums.StatusUsuario.Ausente)
                    && u.Organizacao.Nome == nomeOrganizacao)
                .ToListAsync(ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteUsuarioAptoComId(Guid idUsuario, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.AnyAsync(usuario => usuario.Id.Equals(idUsuario) &&
                (usuario.Status == Domain.Enums.StatusUsuario.Ativo
                    || usuario.Status == Domain.Enums.StatusUsuario.Ausente)
                , ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteUsuarioAptoComEmailNaOrganizacao(string email, string nomeOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.AnyAsync(usuario =>
                usuario.Email.Equals(email) &&
                (usuario.Status == Domain.Enums.StatusUsuario.Ativo || usuario.Status == Domain.Enums.StatusUsuario.Ausente) &&
                usuario.Organizacao.Nome.Equals(nomeOrganizacao)
            , ct),
            cancellationToken
        );
    }

    public async Task<Usuario?> PegarUsuarioAptoPorId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .Include(usuario => usuario.PerfisAcesso)
                .Include(usuario => usuario.Permissoes)
                .Include(usuario => usuario.Organizacao)
                .FirstOrDefaultAsync(usuario => usuario.Id.Equals(id)
                    && (usuario.Status == Domain.Enums.StatusUsuario.Ativo
                        || usuario.Status == Domain.Enums.StatusUsuario.Ausente)
                    , ct),
            cancellationToken
        );
    }

    public async Task<Usuario?> PegarUsuarioPorId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .Include(usuario => usuario.PerfisAcesso)
                .Include(usuario => usuario.Permissoes)
                .Include(usuario => usuario.Organizacao)
                .FirstOrDefaultAsync(usuario => usuario.Id.Equals(id), ct),
            cancellationToken
        );
    }

    public async Task AlterarSenha(Guid id, string novaSenha, CancellationToken cancellationToken)
    {
        var usuario = await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.FindAsync([id, ct], ct),
            cancellationToken
        );
        usuario!.Senha = novaSenha;
    }

    public async Task AlterarSenhaEAtivar(Guid id, string novaSenha, CancellationToken cancellationToken)
    {
        var usuario = await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.FindAsync([id, ct], ct),
            cancellationToken
        );
        usuario!.Senha = novaSenha;
        usuario!.Status = Domain.Enums.StatusUsuario.Ativo;
    }

    public async Task ExcluirDoBanco(Guid id, CancellationToken cancellationToken)
    {
        var usuario = await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.FindAsync([id, ct], ct),
            cancellationToken
        );

        if (usuario is null)
            return;

        RemoverTodosTokensUsuario(id);

        _dbContext.Remove(usuario);
    }

    public async Task<IList<Usuario>?> PegarUsuariosParaAtivacao(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .AsNoTracking()
                .Where(usuario => usuario.Status == Domain.Enums.StatusUsuario.Inativo
                    && !_dbContext.TokensAtivacao.Any(token => token.IdUsuario == usuario.Id))
                .OrderBy(usuario => usuario.DataCriacaoUtc)
                .ToListAsync(ct),
            cancellationToken
        );
    }

    public async Task<IList<Usuario>?> PegarUsuariosParaEnvioEmailAtivacao(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .Include(usuario => usuario.Organizacao)
                .AsNoTracking()
                .Where(usuario => usuario.Status == Domain.Enums.StatusUsuario.Inativo
                    && _dbContext.TokensAtivacao.Any(token =>
                        token.IdUsuario == usuario.Id &&
                        !token.EmailEnviado &&
                        token.DataCriacaoUtc.AddHours(_geradorTokenAtivacao.TempoValidadeHoras) >= DateTime.UtcNow))
                .OrderBy(usuario => usuario.DataCriacaoUtc)
                .ToListAsync(ct),
            cancellationToken
        );
    }

    public async Task<IList<Usuario>?> PegarUsuariosParaEnvioEmailNovaSenha(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Usuarios
                .Include(usuario => usuario.Organizacao)
                .AsNoTracking()
                .Where(usuario => usuario.Status == Domain.Enums.StatusUsuario.Ativo
                    && _dbContext.TokensNovaSenha.Any(token =>
                        token.IdUsuario == usuario.Id &&
                        !token.EmailEnviado &&
                        token.DataCriacaoUtc.AddHours(_geradorTokenNovaSenha.TempoValidadeHoras) >= DateTime.UtcNow))
                .OrderBy(usuario => usuario.DataCriacaoUtc)
                .ToListAsync(ct),
            cancellationToken
        );
    }

    public async Task MudarStatus(Guid id, Domain.Enums.StatusUsuario status, CancellationToken cancellationToken)
    {
        if (status == Domain.Enums.StatusUsuario.Excluido)
        {
            await MarcarComoExcluido(id, cancellationToken);
            return;
        }

        var usuario = await _retryPolicy.ExecuteAsync(
            async (ct) => await _dbContext.Usuarios.FindAsync([id, ct], ct),
            cancellationToken
        );

        if (usuario is null)
            return;

        usuario.Status = status;
    }

    private async Task MarcarComoExcluido(Guid id, CancellationToken cancellationToken)
    {
        var usuario = await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Usuarios.FindAsync([id, ct], ct),
            cancellationToken
        );

        if (usuario is null)
            return;

        RemoverTodosTokensUsuario(id);

        usuario.Status = Domain.Enums.StatusUsuario.Excluido;
    }

    public void RemoverTodosTokensUsuario(Guid id)
    {
        var refreshTokens = _dbContext.RefreshTokens.Where(refreshToken => refreshToken.IdUsuario.Equals(id));
        if (refreshTokens != null && refreshTokens.Any())
            _dbContext.RefreshTokens.RemoveRange(refreshTokens);

        var tokensAtivacao = _dbContext.TokensAtivacao.Where(tokenAtivacao => tokenAtivacao.IdUsuario.Equals(id));
        if (tokensAtivacao != null && tokensAtivacao.Any())
            _dbContext.TokensAtivacao.RemoveRange(tokensAtivacao);

        var tokensNovaSenha = _dbContext.TokensNovaSenha.Where(tokenNovaSenha => tokenNovaSenha.IdUsuario.Equals(id));
        if (tokensNovaSenha != null && tokensNovaSenha.Any())
            _dbContext.TokensNovaSenha.RemoveRange(tokensNovaSenha);
    }
}