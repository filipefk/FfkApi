using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class UsuarioRepositoryBuilder
{
    private readonly Mock<IUsuarioRepository> _usuarioRepository;

    public UsuarioRepositoryBuilder()
    {
        _usuarioRepository = new Mock<IUsuarioRepository>();
    }
    public IUsuarioRepository Build()
    {
        return _usuarioRepository.Object;
    }

    public void SetupExisteUsuarioComEmailReturnsTrue(string email, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.ExisteUsuarioComEmail(email, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupExisteUsuarioComCpfReturnsTrue(string cpf, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.ExisteUsuarioComCpf(cpf, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupExisteUsuarioComCpfReturnsTrue(string cpf, Guid? idOrganizacao, CancellationToken cancellationToken)
    {
        if (idOrganizacao == null)
            _usuarioRepository.Setup(repository => repository.ExisteUsuarioComCpf(cpf, It.IsAny<Guid>(), cancellationToken)).ReturnsAsync(true);
        else
            _usuarioRepository.Setup(repository => repository.ExisteUsuarioComCpf(cpf, idOrganizacao.Value, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarUsuarioAptoPorEmailReturnsUsuario(Usuario usuario, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.PegarUsuarioAptoPorEmail(usuario.Email, cancellationToken)).ReturnsAsync(usuario);
    }

    public void SetupPegarUsuarioAptoPorEmailReturnsUsuario(Usuario usuario, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.PegarUsuarioAptoPorEmail(usuario.Email, idOrganizacao, cancellationToken)).ReturnsAsync(usuario);
    }

    public void SetupPegarUsuarioAptoPorIdReturnsUsuario(Usuario usuario, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.PegarUsuarioAptoPorId(usuario.Id, cancellationToken)).ReturnsAsync(usuario);
    }

    public void SetupPegarUsuarioPorIdReturnsUsuario(Usuario usuario, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.PegarUsuarioPorId(usuario.Id, cancellationToken)).ReturnsAsync(usuario);
    }

    public void SetupPegarUsuariosParaAtivacaoReturnsUsuarios(List<Usuario> usuarios, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.PegarUsuariosParaAtivacao(cancellationToken)).ReturnsAsync(usuarios);
    }

    public void SetupPegarUsuariosParaEnvioEmailAtivacaoReturnsUsuarios(List<Usuario> usuarios, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.PegarUsuariosParaEnvioEmailAtivacao(cancellationToken)).ReturnsAsync(usuarios);
    }

    public void SetupPegarUsuariosParaEnvioEmailNovaSenhaReturnsUsuarios(List<Usuario> usuarios, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.PegarUsuariosParaEnvioEmailNovaSenha(cancellationToken)).ReturnsAsync(usuarios);
    }

    public void SetupExisteUsuarioAptoComEmailNaOrganizacaoReturnsTrue(string email, string nomeOrganizacao, CancellationToken cancellationToken)
    {
        _usuarioRepository.Setup(repository => repository.ExisteUsuarioAptoComEmailNaOrganizacao(email, nomeOrganizacao, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarUsuariosAptosPorEmailsReturnsUsuarios(
        List<Usuario> usuarios,
        CancellationToken cancellationToken)
    {
        var emailsUsuarios = usuarios.Select(usuario => usuario.Email).ToList();
        _usuarioRepository.Setup(repository => repository.PegarUsuariosAptosPorEmails(emailsUsuarios, cancellationToken))
            .ReturnsAsync(usuarios);
    }

    public void SetupPegarUsuariosAptosPorEmailsReturnsUsuarios(
        List<Usuario> usuarios,
        string nomeOrganizacao,
        CancellationToken cancellationToken)
    {
        var emailsUsuarios = usuarios.Select(usuario => usuario.Email).ToList();
        _usuarioRepository.Setup(repository => repository.PegarUsuariosAptosPorEmails(emailsUsuarios, nomeOrganizacao, cancellationToken))
            .ReturnsAsync(usuarios);
    }
}
