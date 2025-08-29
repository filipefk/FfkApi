using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public static class RequestAlterarUsuarioBuilder
{
    public static RequestAlterarUsuario Build(string uniqueSuffix = "")
    {
        return new Faker<RequestAlterarUsuario>()
            .RuleFor(request => request.Id, faker => faker.Random.Guid().ToString())
            .RuleFor(request => request.Nome, faker => faker.Person.FirstName)
            .RuleFor(request => request.Email, (faker, usuario) => faker.Internet.Email(usuario.Nome, uniqueSuffix: uniqueSuffix))
            .RuleFor(request => request.Cpf, fake => fake.Person.CpfSoNumeros())
            .RuleFor(request => request.Telefone, fake => fake.Person.CelularBrasileiro())
            .RuleFor(request => request.Organizacao, fake => fake.NomeEmpresa())
            .RuleFor(request => request.Status, fake => fake.PickRandom(Usuario.StatusPermitidosAoAlterarStatusDeOutroUsuario()));
    }

    public static RequestAlterarUsuario Build(Usuario usuario)
    {
        return new RequestAlterarUsuario
        {
            Id = usuario.Id.ToString(),
            Nome = usuario.Nome,
            Email = usuario.Email,
            Cpf = usuario.Cpf,
            Telefone = usuario.Telefone,
            Organizacao = usuario.Organizacao.Nome,
            Status = usuario.Status.ToString()
        };
    }
}
