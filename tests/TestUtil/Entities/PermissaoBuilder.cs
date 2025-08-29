using Bogus;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class PermissaoBuilder
{
    public static Permissao Build(string? nome = null)
    {
        if (nome == null)
            nome = new Faker().Lorem.Sentence(3);

        return new Faker<Permissao>()
            .RuleFor(permissao => permissao.Id, () => Guid.NewGuid())
            .RuleFor(permissao => permissao.Nome, _ => nome)
            .RuleFor(permissao => permissao.Descricao, fake => fake.Proverbio());
    }

    public static List<Permissao> BuildList(int quant = 3)
    {
        var listaPermissoes = new List<Permissao>();
        for (int i = 0; i < quant; i++)
        {
            listaPermissoes.Add(Build());
        }
        return listaPermissoes;
    }

    public static List<Permissao> BuildList(IList<string> lista)
    {
        return lista
            .Select(nome => Build(nome))
            .ToList();
    }
}
