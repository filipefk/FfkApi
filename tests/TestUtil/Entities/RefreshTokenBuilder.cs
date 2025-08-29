using Bogus;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class RefreshTokenBuilder
{
    public static RefreshToken Build(Usuario? usuario = null)
    {
        if (usuario == null)
            usuario = UsuarioBuilder.Build();

        return new Faker<RefreshToken>()
            .RuleFor(request => request.Id, () => Guid.NewGuid())
            .RuleFor(request => request.Valor, faker => faker.Random.AlphaNumericUrl(24))
            .RuleFor(tokenAtivacao => tokenAtivacao.IdUsuario, () => usuario.Id)
            .RuleFor(tokenAtivacao => tokenAtivacao.Usuario, () => usuario);
    }
}
