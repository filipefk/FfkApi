using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IUsuarioRepository
{
    Task<bool> ExisteUsuarioComEmail(string email, CancellationToken cancellationToken);
    Task<bool> ExisteUsuarioComCpf(string cpf, CancellationToken cancellationToken);
    Task<bool> ExisteUsuarioComCpf(string cpf, Guid idOrganizacao, CancellationToken cancellationToken);
    Task Adicionar(Usuario usuario, CancellationToken cancellationToken);
    Task<Usuario?> PegarUsuarioAptoPorEmail(string email, CancellationToken cancellationToken);
    Task<Usuario?> PegarUsuarioAptoPorEmail(string email, Guid idOrganizacao, CancellationToken cancellationToken);
    Task<bool> ExisteUsuarioAptoComId(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteUsuarioAptoComEmailNaOrganizacao(string email, string nomeOrganizacao, CancellationToken cancellationToken);
    Task<Usuario?> PegarUsuarioAptoPorId(Guid id, CancellationToken cancellationToken);
    Task<Usuario?> PegarUsuarioPorId(Guid id, CancellationToken cancellationToken);
    Task AlterarSenha(Guid id, string novaSenha, CancellationToken cancellationToken);
    Task AlterarSenhaEAtivar(Guid id, string novaSenha, CancellationToken cancellationToken);
    Task ExcluirDoBanco(Guid id, CancellationToken cancellationToken);
    // TODO : O método QuantidadeTotal deve ter um parâmetro de Organizacao
    Task<long> QuantidadeTotal(CancellationToken cancellationToken);
    IQueryable<Usuario> AsQueryable();
    Task<IList<Usuario>?> PegarUsuariosParaAtivacao(CancellationToken cancellationToken);
    Task<IList<Usuario>?> PegarUsuariosParaEnvioEmailAtivacao(CancellationToken cancellationToken);
    Task<IList<Usuario>?> PegarUsuariosParaEnvioEmailNovaSenha(CancellationToken cancellationToken);
    Task MudarStatus(Guid id, Enums.StatusUsuario status, CancellationToken cancellationToken);
    void RemoverTodosTokensUsuario(Guid id);
    Task<IList<Usuario>> PegarUsuariosAptosPorEmails(IList<string> emails, CancellationToken cancellationToken);
    Task<IList<Usuario>> PegarUsuariosAptosPorEmails(IList<string> emails, string nomeOrganizacao, CancellationToken cancellationToken);
}
