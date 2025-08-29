using Bogus;
using FfkApi.Domain.Entities;
using FfkApi.Infrastructure.Security.Credenciais;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class SistemaClienteBuilder
{
    public static SistemaCliente Build()
    {
        return new Faker<SistemaCliente>()
            .RuleFor(sistemaCliente => sistemaCliente.Id, () => Guid.NewGuid())
            .RuleFor(sistemaCliente => sistemaCliente.AppId, () => Guid.NewGuid())
            .RuleFor(request => request.Nome, fake => fake.NomeEmpresa())
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.Senha, _ => new GeradorToken().GerarToken())
            .RuleFor(request => request.Status, _ => FfkApi.Domain.Enums.StatusSistemaCliente.Ativo);
    }
}
