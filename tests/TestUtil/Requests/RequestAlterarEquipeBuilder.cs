using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestAlterarEquipeBuilder
{
    public static RequestAlterarEquipe Build(int quantMembros = 3)
    {
        return new Faker<RequestAlterarEquipe>()
            .RuleFor(request => request.Id, fake => fake.Random.Guid().ToString())
            .RuleFor(request => request.Nome, fake => fake.NomeEquipe())
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.Status, _ => "Ativa")
            .RuleFor(request => request.Membros, _ => RequestCadastrarEquipeBuilder.Membros(quantMembros))
            .RuleFor(request => request.Organizacao, fake => fake.NomeEmpresa());
    }

    public static RequestAlterarEquipe Build(Equipe equipe)
    {
        return new RequestAlterarEquipe
        {
            Id = equipe.Id.ToString(),
            Nome = equipe.Nome,
            Descricao = equipe.Descricao,
            Status = equipe.Status.ToString(),
            Membros = equipe.Membros.Select(RequestMembroEquipeBuilder.Build).ToList(),
            Organizacao = equipe.Organizacao.Nome
        };
    }
}
