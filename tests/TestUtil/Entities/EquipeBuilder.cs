using Bogus;
using FfkApi.Domain.Entities;
using FfkApi.Domain.Enums;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class EquipeBuilder
{
    public static Equipe Build(string? nomeEquipe = null, Organizacao? organizacao = null!)
    {
        nomeEquipe ??= new Faker().NomeEquipe();
        organizacao ??= OrganizacaoBuilder.Build();

        return new Faker<Equipe>()
            .RuleFor(equipe => equipe.Id, _ => Guid.NewGuid())
            .RuleFor(equipe => equipe.Nome, _ => nomeEquipe)
            .RuleFor(equipe => equipe.Descricao, fake => fake.Proverbio())
            .RuleFor(equipe => equipe.Status, _ => StatusEquipe.Ativa)
            .RuleFor(equipe => equipe.IdOrganizacao, _ => organizacao.Id)
            .RuleFor(equipe => equipe.Organizacao, _ => organizacao);
    }
}
