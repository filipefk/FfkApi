using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface ITokenAtivacaoRepository
{
    Task<TokenAtivacao?> PegarTokenAtivacaoPorToken(string tokenAtivacao, CancellationToken cancellationToken);
    Task<TokenAtivacao?> PegarTokenAtivacaoPorUsuario(Guid idUsuario, CancellationToken cancellationToken);
    Task SalvarNovoTokenAtivacao(TokenAtivacao tokenAtivacao, CancellationToken cancellationToken);
    Task RedefinirDataExpiracaoEMarcarParaEnviarNovoEmail(Guid idTokenAtivacao, CancellationToken cancellationToken);
    void ApagarTokensDoUsuario(Guid idUsuario);
}
