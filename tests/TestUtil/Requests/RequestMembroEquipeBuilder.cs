using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;

namespace TestUtil.Requests;

public class RequestMembroEquipeBuilder
{
    public static RequestMembroEquipe Build(bool lider = false)
    {
        return new Faker<RequestMembroEquipe>()
            .RuleFor(request => request.Email, fake => fake.Internet.Email())
            .RuleFor(request => request.Lider, _ => lider);
    }

    public static RequestMembroEquipe Build(MembroEquipe membroEquipe)
    {
        return new RequestMembroEquipe
        {
            Email = membroEquipe.Usuario.Email,
            Lider = membroEquipe.Lider
        };
    }

    public static RequestMembroEquipe Build(Usuario usuario, bool lider = false)
    {
        return new RequestMembroEquipe
        {
            Email = usuario.Email,
            Lider = lider
        };
    }

    public static List<RequestMembroEquipe> BuildList(List<Usuario> usuarios)
    {
        var lista = new List<RequestMembroEquipe>();
        foreach (var usuario in usuarios)
        {
            lista.Add(Build(usuario));
        }
        return lista;
    }
}
