using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace FfkApi.Application.Extension;

public static class FluentValidationExtension
{
    public static IRuleBuilderOptions<T, string> Cpf<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(CpfValido);
    }

    private static bool CpfValido(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11 || !Regex.IsMatch(cpf, @"^\d{11}$") || TodosDigitosIguais(cpf))
            return false;

        Span<int> multiplicadores1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        Span<int> multiplicadores2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        int digito1 = CalcularDigito(cpf.AsSpan(0, 9), multiplicadores1);
        int digito2 = CalcularDigito(cpf.AsSpan(0, 9).ToString() + digito1, multiplicadores2);

        return cpf.EndsWith($"{digito1}{digito2}");
    }

    private static int CalcularDigito(ReadOnlySpan<char> cpf, ReadOnlySpan<int> multiplicadores)
    {
        int soma = 0;
        for (int i = 0; i < multiplicadores.Length; i++)
            soma += (cpf[i] - '0') * multiplicadores[i];

        int resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static int CalcularDigito(string cpf, ReadOnlySpan<int> multiplicadores)
    {
        int soma = 0;
        for (int i = 0; i < multiplicadores.Length; i++)
            soma += (cpf[i] - '0') * multiplicadores[i];

        int resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static bool TodosDigitosIguais(string cpf)
    {
        char firstChar = cpf[0];
        for (int i = 1; i < cpf.Length; i++)
        {
            if (cpf[i] != firstChar)
                return false;
        }
        return true;
    }

    public static IRuleBuilderOptions<T, string> Telefone<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(TelefoneValido);
    }

    private static bool TelefoneValido(string telefone)
    {
        return telefone.Length <= 20 && Regex.IsMatch(telefone, @"^\+?\d+$");
    }

    public static List<string> ToListErros(this ValidationResult validationResult)
    {
        return validationResult.Errors.Select(e => e.ErrorMessage).ToList();
    }
}
