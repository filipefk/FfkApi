using FfkApi.Domain.Entities;

namespace TestUtil.Entities;

public class MembroEquipeBuilder
{
    public static MembroEquipe Build(string? nomeOrganizacao = null, Equipe? equipe = null, Usuario? usuario = null)
    {
        var organizacao = OrganizacaoBuilder.Build(nomeOrganizacao);
        usuario ??= UsuarioBuilder.Build(organizacao: organizacao);
        equipe ??= EquipeBuilder.Build(organizacao: organizacao);

        return new MembroEquipe
        {
            Id = Guid.NewGuid(),
            Lider = false,
            IdEquipe = equipe.Id,
            Equipe = equipe,
            IdUsuario = usuario.Id,
            Usuario = usuario
        };
    }

    public static List<MembroEquipe> BuildList(int quant = 3, string? nomeOrganizacao = null, Equipe? equipe = null)
    {
        var membrosEquipe = new List<MembroEquipe>();
        var organizacao = OrganizacaoBuilder.Build(nomeOrganizacao);
        equipe ??= EquipeBuilder.Build(organizacao: organizacao);
        for (int i = 0; i < quant; i++)
        {
            membrosEquipe.Add(Build(nomeOrganizacao, equipe));
        }
        return membrosEquipe;
    }

}
