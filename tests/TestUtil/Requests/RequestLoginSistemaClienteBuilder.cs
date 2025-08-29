using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using FfkApi.Infrastructure.Security.Credenciais;

namespace TestUtil.Requests;

public class RequestLoginSistemaClienteBuilder
{
    public static RequestLoginSistemaCliente Build()
    {
        return new Faker<RequestLoginSistemaCliente>()
            .RuleFor(request => request.AppId, _ => Guid.NewGuid().ToString())
            .RuleFor(request => request.Senha, _ => new GeradorToken().GerarToken());
    }

    public static RequestLoginSistemaCliente Build(SistemaCliente sistemaCliente)
    {
        return new RequestLoginSistemaCliente
        {
            AppId = sistemaCliente.AppId.ToString(),
            Senha = sistemaCliente.Senha
        };
    }
}
