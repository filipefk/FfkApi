namespace FfkApi.Application.Validators;

public static class DataValidator
{
    public static bool DataValida(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return false;

        if (DateTime.TryParseExact(
            data,
            "dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out _)) return true;

        return false;
    }

    public static bool DataFinalMaiorOuIgualDataInicial(string dataInicial, string dataFinal)
    {
        if (!DataValida(dataInicial) || !DataValida(dataFinal))
            return false;
        DateTime dataInicialDateTime = DateTime.ParseExact(dataInicial, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        DateTime dataFinalDateTime = DateTime.ParseExact(dataFinal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        return dataFinalDateTime >= dataInicialDateTime;
    }
}
