using Bogus;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class IndisponibilidadeBuilder
{
    public static Indisponibilidade Build(Usuario? usuario = null)
    {
        usuario ??= UsuarioBuilder.Build();

        return new Faker<Indisponibilidade>()
            .RuleFor(indisponibilidade => indisponibilidade.Id, _ => Guid.NewGuid())
            .RuleFor(indisponibilidade => indisponibilidade.Descricao, fake => fake.Indisponibilidade())
            .RuleFor(indisponibilidade => indisponibilidade.DataInicial, fake => fake.DateOnlyDate())
            .RuleFor(indisponibilidade => indisponibilidade.DataFinal, (fake, indisponibilidade) => fake.DateOnlyDate(indisponibilidade.DataInicial))
            .RuleFor(indisponibilidade => indisponibilidade.IdUsuario, _ => usuario.Id)
            .RuleFor(indisponibilidade => indisponibilidade.Usuario, _ => usuario);
    }
}
