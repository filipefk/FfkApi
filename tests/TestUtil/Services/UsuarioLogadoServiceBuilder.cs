using FfkApi.Domain.Entities;
using FfkApi.Domain.Services.UsuarioLogado;
using Moq;

namespace TestUtil.Services;

public class UsuarioLogadoServiceBuilder
{
    public static IUsuarioLogadoService Build(Usuario usuario, CancellationToken cancellationToken)
    {
        var UsuarioLogadoMock = new Mock<IUsuarioLogadoService>();
        UsuarioLogadoMock.Setup(usuarioLogado => usuarioLogado.PegarUsuarioLogadoAtivo(cancellationToken)).ReturnsAsync(usuario);

        return UsuarioLogadoMock.Object;
    }
}
