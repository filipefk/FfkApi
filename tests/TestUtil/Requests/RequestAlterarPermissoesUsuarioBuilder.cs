using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using FfkApi.Domain.Extension;

namespace TestUtil.Requests;

public class RequestAlterarPermissoesUsuarioBuilder
{
    public static RequestAlterarPermissoesUsuario Build()
    {
        return new Faker<RequestAlterarPermissoesUsuario>()
            .RuleFor(request => request.Id, faker => faker.Random.Guid().ToString())
            .RuleFor(request => request.PerfisAcesso, fake => fake.Make(fake.Random.Int(1, 3), () => fake.Lorem.Word()))
            .RuleFor(request => request.Permissoes, fake => fake.Make(fake.Random.Int(1, 3), () => fake.Lorem.Sentence(3)));
    }

    public static RequestAlterarPermissoesUsuario Build(Usuario usuario)
    {
        return new RequestAlterarPermissoesUsuario
        {
            Id = usuario.Id.ToString(),
            PerfisAcesso = usuario.PerfisAcesso.ToListNome(),
            Permissoes = usuario.Permissoes.ToListNome()
        };
    }
}
