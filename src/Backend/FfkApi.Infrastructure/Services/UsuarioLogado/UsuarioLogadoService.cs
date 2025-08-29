using FfkApi.Domain.Entities;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FfkApi.Infrastructure.Services.UsuarioLogado;

public class UsuarioLogadoService : IUsuarioLogadoService
{
    private readonly IValidadorTokenUsuario _validadorTokenUsuario;
    private readonly FfkApiDbContext _dbContext;
    private readonly ITokenRecebido _tokenRecebido;

    public UsuarioLogadoService(FfkApiDbContext dbContext,
        ITokenRecebido tokenRecebido,
        IValidadorTokenUsuario validadorTokenUsuario)
    {
        _dbContext = dbContext;
        _tokenRecebido = tokenRecebido;
        _validadorTokenUsuario = validadorTokenUsuario;
    }

    public async Task<Usuario> PegarUsuarioLogadoAtivo(CancellationToken cancellationToken)
    {
        var token = _tokenRecebido.Token();

        var idUsuario = _validadorTokenUsuario.PegarIdUsuario(token);

        return await _dbContext
            .Usuarios
            .Include(usuario => usuario.PerfisAcesso)
            .Include(usuario => usuario.Permissoes)
            .Include(usuario => usuario.Organizacao)
            .AsNoTracking()
            .FirstAsync(u => u.Id.Equals(idUsuario)
                && (u.Status == Domain.Enums.StatusUsuario.Ativo
                    || u.Status == Domain.Enums.StatusUsuario.Ausente)
                , cancellationToken);
    }

    public async Task<Usuario?> PegarUsuarioDoTokenEnviado(CancellationToken cancellationToken)
    {
        var token = _tokenRecebido.Token();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var idUsuario = _validadorTokenUsuario.PegarIdUsuario(token);

        return await _dbContext
            .Usuarios
            .AsNoTracking()
            .FirstAsync(u => u.Id.Equals(idUsuario), cancellationToken);
    }

}
