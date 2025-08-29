using Bogus;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class PerfilAcessoBuilder
{
    public static PerfilAcesso Build(string? nome = null, int? quantPermissoes = null)
    {
        List<Permissao>? listaPermissoes = null;

        if (nome == null)
            nome = new Faker().Lorem.Word();

        if (quantPermissoes != null)
            listaPermissoes = PermissaoBuilder.BuildList(quantPermissoes.Value);

        return new Faker<PerfilAcesso>()
            .RuleFor(perfilAcesso => perfilAcesso.Id, () => Guid.NewGuid())
            .RuleFor(perfilAcesso => perfilAcesso.Nome, _ => nome)
            .RuleFor(perfilAcesso => perfilAcesso.Descricao, fake => fake.Proverbio())
            .RuleFor(perfilAcesso => perfilAcesso.Permissoes, _ => listaPermissoes);
    }

    public static List<PerfilAcesso> BuildList(int quant = 3)
    {
        var listaPerfilAcesso = new List<PerfilAcesso>();
        for (int i = 0; i < quant; i++)
        {
            listaPerfilAcesso.Add(Build());
        }
        return listaPerfilAcesso;
    }

    public static List<PerfilAcesso> BuildList(IList<string> lista)
    {
        return lista
            .Select(nome => Build(nome))
            .ToList();
    }

}
