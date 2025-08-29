using FfkApi.Domain.Extension;

namespace FfkApi.Domain.Enums;

public static class EnumUtil
{
    public static bool TextoEnumValido<T>(string enumTexto) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(enumTexto))
            return false;

        return Enum.TryParse<T>(enumTexto.Trim(), true, out _);
    }

    public static string PegarNomesEnumSeparadosPorVirgula<T>(List<string>? removerEstes = null) where T : Enum
    {
        return PegarListaNomesEnum<T>(removerEstes).ListaSepadadaPorVirgula();
    }

    public static T ConverterTextoParaEnum<T>(string enumTexto) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(enumTexto))
            return default;

        return Enum.TryParse<T>(enumTexto.Trim(), true, out var result) ? result : default;
    }

    public static List<string> PegarListaNomesEnum<T>(List<string>? removerEstes = null) where T : Enum
    {
        var nameList = Enum.GetNames(typeof(T));

        if (removerEstes is { Count: > 0 })
        {
            var removerSet = new HashSet<string>(removerEstes, StringComparer.OrdinalIgnoreCase);
            nameList = nameList.Where(name => !removerSet.Contains(name)).ToArray();
        }

        return nameList.ToList();
    }

    public static List<T> PegarListaEnum<T>(List<T>? removerEstes = null) where T : Enum
    {
        var enumList = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        if (removerEstes is { Count: > 0 })
        {
            var removerSet = new HashSet<T>(removerEstes);
            enumList = enumList.Where(e => !removerSet.Contains(e)).ToList();
        }

        return enumList;
    }
}
