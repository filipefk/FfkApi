using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Extension;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestCadastrarUsuarioBuilder
{
    public static RequestCadastrarUsuario Build()
    {
        return new Faker<RequestCadastrarUsuario>()
            .RuleFor(request => request.Nome, faker => faker.Person.FirstName)
            .RuleFor(request => request.Email, (faker, usuario) => faker.Internet.Email(usuario.Nome))
            .RuleFor(request => request.Cpf, fake => fake.Person.CpfSoNumeros())
            .RuleFor(request => request.Telefone, fake => fake.Person.CelularBrasileiro())
            .RuleFor(request => request.Organizacao, fake => fake.NomeEmpresa())
            .RuleFor(request => request.PerfisAcesso, fake => fake.Make(fake.Random.Int(1, 3), () => fake.Lorem.Word()))
            .RuleFor(request => request.Permissoes, fake => fake.Make(fake.Random.Int(1, 3), () => fake.Lorem.Sentence(3)));
    }

    public static RequestCadastrarUsuario Build(FfkApi.Domain.Entities.Usuario usuario)
    {
        return new RequestCadastrarUsuario
        {
            Nome = usuario.Nome,
            Email = usuario.Email,
            Cpf = usuario.Cpf,
            Telefone = usuario.Telefone,
            Organizacao = usuario.Organizacao.Nome,
            PerfisAcesso = usuario.PerfisAcesso.ToListNome(),
            Permissoes = usuario.Permissoes.ToListNome()
        };
    }
}
