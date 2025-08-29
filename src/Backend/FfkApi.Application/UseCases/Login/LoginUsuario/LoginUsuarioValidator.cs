using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Login.LoginUsuario;

public class LoginUsuarioValidator : AbstractValidator<RequestLoginUsuario>
{
    public LoginUsuarioValidator()
    {
        RuleFor(request => request.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_VAZIO);
        RuleFor(request => request.Senha).NotEmpty().WithMessage(ResourceMessagesException.SENHA_VAZIA);
    }
}
