using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Usuario.Ativar;

public class AtivarUsuarioValidator : AbstractValidator<RequestAtivarUsuario>
{
    public AtivarUsuarioValidator()
    {
        RuleFor(request => request.TokenAtivacao).NotEmpty().WithMessage(ResourceMessagesException.TOKEN_ATIVACAO_VAZIO);
        RuleFor(request => request.Senha).NotEmpty().WithMessage(ResourceMessagesException.SENHA_VAZIA);
        When(request => !string.IsNullOrWhiteSpace(request.Senha), () =>
        {
            RuleFor(request => request.Senha).SetValidator(new SenhaValidator<RequestAtivarUsuario>()!);
        });
    }
}

