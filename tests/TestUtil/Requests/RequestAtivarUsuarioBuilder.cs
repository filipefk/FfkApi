using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestAtivarUsuarioBuilder
{
    public static RequestAtivarUsuario Build(string tokenAtivacao = null!, int passwordLength = 10)
    {
        if (tokenAtivacao == null)
            tokenAtivacao = new Faker().Random.AlphaNumericUrl(43);

        return new Faker<RequestAtivarUsuario>()
            .RuleFor(request => request.TokenAtivacao, () => tokenAtivacao)
            .RuleFor(request => request.Nome, faker => faker.Person.FirstName)
            .RuleFor(request => request.Email, (faker, usuario) => faker.Internet.Email(usuario.Nome))
            .RuleFor(request => request.Cpf, fake => fake.Person.CpfSoNumeros())
            .RuleFor(request => request.Senha, fake => fake.Internet.Senha(passwordLength));
    }

    public static RequestAtivarUsuario Build(Usuario usuario, string tokenAtivacao = null!)
    {
        if (tokenAtivacao == null)
            tokenAtivacao = new Faker().Random.AlphaNumericUrl(43);

        return new RequestAtivarUsuario
        {
            TokenAtivacao = tokenAtivacao,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Cpf = usuario.Cpf,
            Senha = new Faker().Internet.Senha(),
        };
    }
}
