using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Enums;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestAlterarFeedBuilder
{
    public static RequestAlterarFeed Build()
    {
        return new Faker<RequestAlterarFeed>()
            .RuleFor(request => request.Id, fake => fake.Random.Guid().ToString())
            .RuleFor(request => request.Nome, fake => fake.Lorem.Sentence(fake.Random.Int(1, 3)))
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.PalavrasChave, fake => string.Join(", ", fake.Lorem.Words(5)))
            .RuleFor(request => request.Status, fake => fake.PickRandom(EnumUtil.PegarListaNomesEnum<StatusFeed>(["Indefinido"])))
            .RuleFor(request => request.VisibilidadeUsuarios, fake => fake.Make(fake.Random.Int(1, 3), () => fake.Internet.Email()))
            .RuleFor(request => request.VisibilidadeEquipes, fake => fake.Make(fake.Random.Int(1, 3), () => fake.Person.FirstName))
            .RuleFor(request => request.ExpiraEm, fake => fake.Date.Future(1, DateTime.Now).ToString("dd/MM/yyyy"))
            .RuleFor(request => request.Organizacao, fake => fake.NomeEmpresa());
    }

    public static RequestAlterarFeed Build(FfkApi.Domain.Entities.Feed feed)
    {
        return new RequestAlterarFeed
        {
            Id = feed.Id.ToString(),
            Nome = feed.Nome,
            Descricao = feed.Descricao,
            PalavrasChave = feed.PalavrasChave,
            Status = feed.Status.ToString(),
            VisibilidadeUsuarios = feed.VisibilidadeUsuarios.Select(u => u.Email).ToList(),
            VisibilidadeEquipes = feed.VisibilidadeEquipes.Select(e => e.Nome).ToList(),
            ExpiraEm = feed.ExpiraEm?.ToString("dd/MM/yyyy"),
            Organizacao = feed.Organizacao.Nome
        };
    }
}