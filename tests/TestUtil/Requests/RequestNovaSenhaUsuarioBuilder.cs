using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestNovaSenhaUsuarioBuilder
{
    public static RequestNovaSenhaUsuario Build(string tokenNovaSenha = null!, int passwordLength = 10)
    {
        if (tokenNovaSenha == null)
            tokenNovaSenha = new Faker().Random.AlphaNumericUrl(43);

        return new Faker<RequestNovaSenhaUsuario>()
            .RuleFor(request => request.TokenNovaSenha, () => tokenNovaSenha)
            .RuleFor(request => request.Nome, faker => faker.Person.FirstName)
            .RuleFor(request => request.Email, (faker, usuario) => faker.Internet.Email(usuario.Nome))
            .RuleFor(request => request.Cpf, fake => fake.Person.CpfSoNumeros())
            .RuleFor(request => request.NovaSenha, fake => fake.Internet.Senha(passwordLength));
    }

    public static RequestNovaSenhaUsuario Build(Usuario usuario, string tokenNovaSenha = null!)
    {
        if (tokenNovaSenha == null)
            tokenNovaSenha = new Faker().Random.AlphaNumericUrl(43);

        return new RequestNovaSenhaUsuario
        {
            TokenNovaSenha = tokenNovaSenha,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Cpf = usuario.Cpf,
            NovaSenha = new Faker().Internet.Senha(),
        };
    }
}
