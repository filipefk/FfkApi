using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FfkApi.Api.Converters;

public class StringConverter : JsonConverter<string>
{
    private static readonly Regex _espacosEmBrancoRegex = new(@"\s+", RegexOptions.Compiled);

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var valor = reader.GetString();
        if (valor is null)
            return null;
        valor = valor.Trim();
        if (valor.Length == 0)
            return valor;
        return _espacosEmBrancoRegex.Replace(valor, " ");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => writer.WriteStringValue(value);
}
