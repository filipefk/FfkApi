using Bogus;
using FfkApi.Communication.Requests;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestAlterarAnexoBuilder
{
    public static RequestAlterarAnexo Build()
    {
        return new Faker<RequestAlterarAnexo>()
            .RuleFor(request => request.Id, fake => fake.Random.Guid().ToString())
            .RuleFor(request => request.Nome, fake => fake.Lorem.Word())
            .RuleFor(request => request.Descricao, fake => fake.Proverbio());
    }

    public static RequestAlterarAnexo Build(FfkApi.Domain.Entities.Anexo anexo)
    {
        return new RequestAlterarAnexo
        {
            Id = anexo.Id.ToString(),
            Nome = anexo.Nome,
            Descricao = anexo.Descricao
        };
    }
}
