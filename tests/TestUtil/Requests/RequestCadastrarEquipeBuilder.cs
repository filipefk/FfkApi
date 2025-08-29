using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestCadastrarEquipeBuilder
{
    public static RequestCadastrarEquipe Build(int quantMembros = 3)
    {
        return new Faker<RequestCadastrarEquipe>()
            .RuleFor(request => request.Nome, fake => fake.NomeEquipe())
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.Status, _ => "Ativa")
            .RuleFor(request => request.Membros, _ => Membros(quantMembros))
            .RuleFor(request => request.Organizacao, fake => fake.NomeEmpresa());
    }

    public static RequestCadastrarEquipe Build(Equipe equipe)
    {
        return new RequestCadastrarEquipe
        {
            Nome = equipe.Nome,
            Descricao = equipe.Descricao,
            Status = equipe.Status.ToString(),
            Membros = equipe.Membros.Select(RequestMembroEquipeBuilder.Build).ToList(),
            Organizacao = equipe.Organizacao.Nome
        };
    }

    public static List<RequestMembroEquipe> Membros(int quant = 3)
    {
        var faker = new Faker();
        var membros = faker.Make(quant, () => RequestMembroEquipeBuilder.Build()).ToList();
        if (membros.Count > 0)
        {
            membros[0].Lider = true;
        }
        return membros;
    }
}
