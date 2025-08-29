using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Infrastructure.Security.Credenciais;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestAlterarSistemaClienteBuilder
{
    public static RequestAlterarSistemaCliente Build()
    {
        return new Faker<RequestAlterarSistemaCliente>()
            .RuleFor(request => request.Id, fake => fake.Random.Guid().ToString())
            .RuleFor(request => request.AppId, fake => fake.Random.Guid().ToString())
            .RuleFor(request => request.Nome, fake => fake.NomeEmpresa())
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.Senha, _ => new GeradorToken().GerarToken())
            .RuleFor(request => request.Status, fake => fake.PickRandom(new[] { "Ativo", "Inativo" }));
    }

    public static RequestAlterarSistemaCliente Build(FfkApi.Domain.Entities.SistemaCliente sistemaCliente)
    {
        return new RequestAlterarSistemaCliente
        {
            Id = sistemaCliente.Id.ToString(),
            AppId = sistemaCliente.AppId.ToString(),
            Nome = sistemaCliente.Nome,
            Descricao = sistemaCliente.Descricao,
            Senha = sistemaCliente.Senha,
            Status = sistemaCliente.Status.ToString()
        };
    }
}
