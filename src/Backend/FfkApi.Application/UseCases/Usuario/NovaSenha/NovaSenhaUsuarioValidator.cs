using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Usuario.NovaSenha;

public class NovaSenhaUsuarioValidator : AbstractValidator<RequestNovaSenhaUsuario>
{
    public NovaSenhaUsuarioValidator()
    {
        RuleFor(request => request.TokenNovaSenha).NotEmpty().WithMessage(ResourceMessagesException.TOKEN_NOVA_SENHA_VAZIO);
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_VAZIO);
        RuleFor(request => request.Cpf).NotEmpty().WithMessage(ResourceMessagesException.CPF_VAZIO);
        RuleFor(request => request.NovaSenha).NotEmpty().WithMessage(ResourceMessagesException.SENHA_VAZIA);
        When(request => !string.IsNullOrWhiteSpace(request.NovaSenha), () =>
        {
            RuleFor(request => request.NovaSenha).SetValidator(new SenhaValidator<RequestNovaSenhaUsuario>()!);
        });
    }
}
