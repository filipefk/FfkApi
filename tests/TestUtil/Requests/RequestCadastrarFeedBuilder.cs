using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Enums;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestCadastrarFeedBuilder
{
    public static RequestCadastrarFeed Build()
    {
        return new Faker<RequestCadastrarFeed>()
            .RuleFor(request => request.Nome, fake => fake.Lorem.Sentence(fake.Random.Int(1, 3)))
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.PalavrasChave, fake => string.Join(", ", fake.Lorem.Words(5)))
            .RuleFor(request => request.Status, fake => fake.PickRandom(EnumUtil.PegarListaNomesEnum<StatusFeed>(["Indefinido"])))
            .RuleFor(request => request.VisibilidadeUsuarios, fake => fake.Make(fake.Random.Int(1, 3), () => fake.Internet.Email()))
            .RuleFor(request => request.VisibilidadeEquipes, fake => fake.Make(fake.Random.Int(1, 3), () => fake.NomeEquipe()))
            .RuleFor(request => request.ExpiraEm, fake => fake.Date.Future(1, DateTime.Now).ToString("dd/MM/yyyy"))
            .RuleFor(request => request.Organizacao, fake => fake.NomeEmpresa());
    }

    public static RequestCadastrarFeed Build(FfkApi.Domain.Entities.Feed feed)
    {
        return new RequestCadastrarFeed
        {
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
