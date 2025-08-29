namespace TestUtil.InlineData;

public class ListaStringNulaVaziaInlineData
{
    public static IEnumerable<List<string>?> ListaStringNulaVazia()
    {
        yield return null;
        yield return new List<string>();
    }
}
