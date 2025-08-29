using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Anexo.Alterar;

public class AlterarAnexoValidator : AbstractValidator<RequestAlterarAnexo>
{
    public AlterarAnexoValidator()
    {
        RuleFor(request => request.Id).NotEmpty().WithMessage(ResourceMessagesException.ID_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Id), () =>
        {
            RuleFor(request => request.Id).Must(IdValidator.IdEstaValido).WithMessage(ResourceMessagesException.ID_INVALIDO);
        });
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Descricao).NotEmpty().WithMessage(ResourceMessagesException.DESCRICAO_VAZIA);
    }
}
