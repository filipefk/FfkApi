namespace FfkApi.Domain.Extension;

public static class EnumerableExtension
{
    public static bool EquivalenteA<T>(this IEnumerable<T> primeira, IEnumerable<T> segunda)
    {
        if ((primeira == null || !primeira.Any()) && (segunda == null || !segunda.Any()))
            return true;

        if (primeira == null && segunda != null)
            return false;

        if (primeira != null && segunda == null)
            return false;

        if (primeira!.Count() != segunda!.Count())
            return false;

        var primeiraLista = primeira!.ToList();
        var segundaLista = segunda!.ToList();

        if (primeiraLista.Count != segundaLista.Count)
            return false;

        var comparador = EqualityComparer<T>.Default;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        var firstGrouped = primeiraLista.GroupBy(x => x, comparador).ToDictionary(g => g.Key, g => g.Count(), comparador);
        var secondGrouped = segundaLista.GroupBy(x => x, comparador).ToDictionary(g => g.Key, g => g.Count(), comparador);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        return firstGrouped.Count == secondGrouped.Count &&
               firstGrouped.All(kvp => secondGrouped.TryGetValue(kvp.Key, out var count) && count == kvp.Value);
    }

    public static bool ListaVazia<T>(this IEnumerable<T> lista)
    {
        return lista == null || !lista.Any();
    }

    public static bool ListaNullOrWhiteSpace(this IEnumerable<string> lista)
    {
        return lista == null || !lista.Any() || lista.All(string.IsNullOrWhiteSpace);
    }

    public static string ListaSepadadaPorVirgula(this IEnumerable<string> lista)
    {
        if (lista == null || !lista.Any())
            return string.Empty;

        return string.Join(", ", lista.Where(nome => !string.IsNullOrWhiteSpace(nome)));
    }

    public static List<string>? ToListNome<T>(this ICollection<T> lista)
    {
        if (lista == null)
            return null;

        if (lista.Count == 0)
            return [];

        var prop = typeof(T).GetProperty("Nome");
        if (prop == null)
            return null;

        return lista
            .Select(item => prop.GetValue(item)?.ToString() ?? string.Empty)
            .ToList();
    }

    public static bool ListaStringTemSoUmItem(this IEnumerable<string> lista, string item)
    {
        return lista != null &&
               lista.Count() == 1 &&
               lista.First().Equals(item);
    }
}
