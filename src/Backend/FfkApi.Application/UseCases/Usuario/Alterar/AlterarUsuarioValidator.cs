using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Usuario.Alterar;

public class AlterarUsuarioValidator : AbstractValidator<RequestAlterarUsuario>
{
    public AlterarUsuarioValidator()
    {
        RuleFor(request => request.Id).NotEmpty().WithMessage(ResourceMessagesException.ID_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Id), () =>
        {
            RuleFor(request => request.Id).Must(IdValidator.IdEstaValido).WithMessage(ResourceMessagesException.ID_INVALIDO);
        });
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
        When(request => !string.IsNullOrWhiteSpace(request.Telefone), () =>
        {
            RuleFor(request => request.Telefone!).Telefone().WithMessage(ResourceMessagesException.TELEFONE_INVALIDO);
        });
        RuleFor(request => request.Status).NotEmpty().WithMessage(ResourceMessagesException.STATUS_VAZIO);
    }
}