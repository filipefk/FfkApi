using FfkApi.Exceptions;
using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace FfkApi.Application.Validators;

public class SenhaValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "SenhaValidator";

    public override bool IsValid(ValidationContext<T> context, string senha)
    {
        if (string.IsNullOrWhiteSpace(senha)
            || senha.Length < 8
            || !Regex.IsMatch(senha, @"[A-Z]")
            || !Regex.IsMatch(senha, @"[a-z]")
            || !Regex.IsMatch(senha, @"[\d]")
            || !Regex.IsMatch(senha, @"[\W_]"))
        {
            context.MessageFormatter.AppendArgument("MensagemErro", ResourceMessagesException.SENHA_INVALIDA);
            return false;
        }

        return true;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "{MensagemErro}";
}
