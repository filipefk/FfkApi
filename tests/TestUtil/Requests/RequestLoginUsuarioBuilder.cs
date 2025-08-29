using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestLoginUsuarioBuilder
{
    public static RequestLoginUsuario Build(int passwordLength = 10)
    {
        return new Faker<RequestLoginUsuario>()
            .RuleFor(request => request.Email, (faker, usuario) => faker.Internet.Email())
            .RuleFor(request => request.Senha, fake => fake.Internet.Senha(passwordLength));
    }

    public static RequestLoginUsuario Build(Usuario usuario)
    {
        var senha = usuario.Senha;

        if (string.IsNullOrWhiteSpace(senha))
            senha = new Faker().Internet.Senha();

        return new RequestLoginUsuario
        {
            Email = usuario.Email,
            Senha = senha
        };
    }
}
