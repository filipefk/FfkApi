namespace TestUtil.Cpf;

public static class CpfUtil
{
    public static string GerarCpfSoNumerosValido()
    {
        int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var random = new Random();
        string seed = string.Join("", Enumerable.Range(0, 9).Select(_ => random.Next(0, 10)));
        int sum = multiplier1.Select((m, i) => m * int.Parse(seed[i].ToString())).Sum();
        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        seed += digit1;
        sum = multiplier2.Select((m, i) => m * int.Parse(seed[i].ToString())).Sum();
        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return $"{seed}{digit2}";
    }
}
