using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Login.LoginSistema;

public class LoginSistemaClienteValidator : AbstractValidator<RequestLoginSistemaCliente>
{
    public LoginSistemaClienteValidator()
    {
        RuleFor(request => request.AppId).NotEmpty().WithMessage(ResourceMessagesException.APP_ID_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.AppId), () =>
        {
            RuleFor(request => request.AppId).Must(IdValidator.IdEstaValido).WithMessage(ResourceMessagesException.APP_ID_INVALIDO);
        });
        RuleFor(request => request.Senha).NotEmpty().WithMessage(ResourceMessagesException.SENHA_VAZIA);
    }
}
