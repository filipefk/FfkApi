using Bogus;
using FfkApi.Domain.Entities;
using FfkApi.Domain.Enums;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class FeedBuilder
{
    public static Feed Build(Organizacao? organizacao = null!)
    {
        organizacao ??= OrganizacaoBuilder.Build();

        return new Faker<Feed>()
            .RuleFor(feed => feed.Id, () => Guid.NewGuid())
            .RuleFor(feed => feed.Nome, fake => fake.Lorem.Sentence(fake.Random.Int(1, 3)))
            .RuleFor(feed => feed.Descricao, fake => fake.Proverbio())
            .RuleFor(feed => feed.PalavrasChave, fake => string.Join(", ", fake.Lorem.Words(5)))
            .RuleFor(feed => feed.Status, fake => fake.PickRandomEnum([StatusFeed.Indefinido]))
            .RuleFor(feed => feed.VisibilidadeUsuarios, _ => [UsuarioBuilder.Build()])
            .RuleFor(feed => feed.VisibilidadeEquipes, _ => [EquipeBuilder.Build()])
            .RuleFor(feed => feed.ExpiraEm, fake => DateOnly.FromDateTime(fake.Date.Future(1, DateTime.Now)))
            .RuleFor(feed => feed.Organizacao, _ => organizacao);
    }
}
