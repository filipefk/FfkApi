using FfkApi.Application.Extension;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Token.TokenNovaSenha;

public class NovoTokenNovaSenhaValidator : AbstractValidator<RequestNovoTokenNovaSenha>
{
    public NovoTokenNovaSenhaValidator()
    {
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Email), () =>
        {
            RuleFor(request => request.Email).EmailAddress().WithMessage(ResourceMessagesException.EMAIL_INVALIDO);
        });
        RuleFor(request => request.Cpf).NotEmpty().WithMessage(ResourceMessagesException.CPF_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Cpf), () =>
        {
            RuleFor(request => request.Cpf!).Cpf().WithMessage(ResourceMessagesException.CPF_INVALIDO);
        });
    }
}
