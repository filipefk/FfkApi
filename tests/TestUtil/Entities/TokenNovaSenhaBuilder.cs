using Bogus;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class TokenNovaSenhaBuilder
{
    public static TokenNovaSenha Build(Usuario? usuario = null)
    {
        if (usuario == null)
            usuario = UsuarioBuilder.Build();

        return new Faker<TokenNovaSenha>()
            .RuleFor(tokenAtivacao => tokenAtivacao.Id, () => Guid.NewGuid())
            .RuleFor(tokenAtivacao => tokenAtivacao.Valor, fake => fake.Random.AlphaNumericUrl(43))
            .RuleFor(tokenAtivacao => tokenAtivacao.IdUsuario, () => usuario.Id)
            .RuleFor(tokenAtivacao => tokenAtivacao.Usuario, () => usuario);
    }
}
