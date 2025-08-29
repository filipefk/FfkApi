using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Infrastructure.Security.Credenciais;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestCadastrarSistemaClienteBuilder
{
    public static RequestCadastrarSistemaCliente Build()
    {
        return new Faker<RequestCadastrarSistemaCliente>()
            .RuleFor(request => request.Nome, fake => fake.NomeEmpresa())
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.Senha, _ => new GeradorToken().GerarToken())
            .RuleFor(request => request.Status, _ => "Ativo");
    }

    public static RequestCadastrarSistemaCliente Build(FfkApi.Domain.Entities.SistemaCliente sistemaCliente)
    {
        return new RequestCadastrarSistemaCliente
        {
            Nome = sistemaCliente.Nome,
            Descricao = sistemaCliente.Descricao,
            Senha = sistemaCliente.Senha,
            Status = sistemaCliente.Status.ToString()
        };
    }
}
