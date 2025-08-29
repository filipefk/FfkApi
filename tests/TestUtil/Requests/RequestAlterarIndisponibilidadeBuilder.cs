using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestAlterarIndisponibilidadeBuilder
{
    public static RequestAlterarIndisponibilidade Build()
    {
        return new Faker<RequestAlterarIndisponibilidade>()
            .RuleFor(request => request.Id, fake => fake.Random.Guid().ToString())
            .RuleFor(request => request.Descricao, fake => fake.Indisponibilidade())
            .RuleFor(request => request.DataInicial, fake => fake.DateOnlyString())
            .RuleFor(request => request.DataFinal, (fake, request) => fake.DateOnlyString(request.DataInicial!))
            .RuleFor(request => request.Usuario, fake => fake.Internet.Email());
    }

    public static RequestAlterarIndisponibilidade Build(Indisponibilidade indisponibilidade)
    {
        return new RequestAlterarIndisponibilidade
        {
            Id = indisponibilidade.Id.ToString(),
            Descricao = indisponibilidade.Descricao,
            DataInicial = indisponibilidade.DataInicial.ToString("dd/MM/yyyy"),
            DataFinal = indisponibilidade.DataFinal.ToString("dd/MM/yyyy"),
            Usuario = indisponibilidade.Usuario.Email
        };
    }
}
