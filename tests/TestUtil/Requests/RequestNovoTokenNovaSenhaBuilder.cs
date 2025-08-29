using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestNovoTokenNovaSenhaBuilder
{
    public static RequestNovoTokenNovaSenha Build(string uniqueSuffix = "")
    {
        return new Faker<RequestNovoTokenNovaSenha>()
            .RuleFor(request => request.Nome, faker => faker.Person.FirstName)
            .RuleFor(request => request.Email, (faker, usuario) => faker.Internet.Email(usuario.Nome, uniqueSuffix: uniqueSuffix))
            .RuleFor(request => request.Cpf, fake => fake.Person.CpfSoNumeros());
    }

    public static RequestNovoTokenNovaSenha Build(Usuario usuario)
    {
        return new RequestNovoTokenNovaSenha
        {
            Nome = usuario.Nome,
            Email = usuario.Email,
            Cpf = usuario.Cpf,
        };
    }
}
