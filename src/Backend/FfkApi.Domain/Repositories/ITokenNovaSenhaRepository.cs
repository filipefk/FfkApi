using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface ITokenNovaSenhaRepository
{
    Task<TokenNovaSenha?> PegarTokenNovaSenhaPorToken(string tokenNovaSenha, CancellationToken cancellationToken);
    Task<TokenNovaSenha?> PegarTokenNovaSenhaPorUsuario(Guid idUsuario, CancellationToken cancellationToken);
    Task SalvarNovoTokenNovaSenha(TokenNovaSenha tokenNovaSenha, CancellationToken cancellationToken);
    void ApagarTokensDoUsuario(Guid idUsuario);
}
