using FfkApi.Domain.Enums;

namespace TestUtil.InlineData;

public static class StatusAoAlterarStatusUsuarioInlineData
{
    public static IEnumerable<string> ListaPermitidaAlterarStatusDeOutroUsuario()
    {
        var lista = FfkApi.Domain.Entities.Usuario.StatusPermitidosAoAlterarStatusDeOutroUsuario();
        foreach (var status in lista)
        {
            yield return status;
        }
    }

    public static IEnumerable<string> ListaInvalidaAlterarStatusDeOutroUsuario()
    {
        var listaCompleta = EnumUtil.PegarListaNomesEnum<StatusUsuario>();
        var listaPermitida = FfkApi.Domain.Entities.Usuario.StatusPermitidosAoAlterarStatusDeOutroUsuario();

        var listaInvalida = listaCompleta.Where(status => !listaPermitida.Contains(status)).ToList();

        listaInvalida.AddRange(["ValorInexistente", "QualquerLixo", "GhdgTGakkd"]);

        foreach (var status in listaInvalida)
        {
            yield return status;
        }
    }
}
