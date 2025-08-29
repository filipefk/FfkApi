using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Usuario.TrocarSenha;

public class TrocarSenhaValidator : AbstractValidator<RequestTrocarSenha>
{
    public TrocarSenhaValidator()
    {
        RuleFor(request => request.SenhaAntiga).NotEmpty().WithMessage(ResourceMessagesException.SENHA_VAZIA);
        RuleFor(request => request.NovaSenha).NotEmpty().WithMessage(ResourceMessagesException.SENHA_VAZIA);
        When(request => !string.IsNullOrWhiteSpace(request.NovaSenha), () =>
        {
            RuleFor(request => request.NovaSenha).SetValidator(new SenhaValidator<RequestTrocarSenha>()!);
        });
    }
}