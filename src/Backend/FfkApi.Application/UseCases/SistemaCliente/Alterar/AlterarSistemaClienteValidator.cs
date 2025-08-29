using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.SistemaCliente.Alterar;

public class AlterarSistemaClienteValidator : AbstractValidator<RequestAlterarSistemaCliente>
{
    public AlterarSistemaClienteValidator()
    {
        RuleFor(request => request.Id).NotEmpty().WithMessage(ResourceMessagesException.ID_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Id), () =>
        {
            RuleFor(request => request.Id).Must(IdValidator.IdEstaValido).WithMessage(ResourceMessagesException.ID_INVALIDO);
        });
        RuleFor(request => request.AppId).NotEmpty().WithMessage(ResourceMessagesException.APP_ID_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.AppId), () =>
        {
            RuleFor(request => request.AppId).Must(IdValidator.IdEstaValido).WithMessage(ResourceMessagesException.APP_ID_INVALIDO);
        });
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Descricao).NotEmpty().WithMessage(ResourceMessagesException.DESCRICAO_VAZIA);
        RuleFor(request => request.Senha).NotEmpty().WithMessage(ResourceMessagesException.SENHA_VAZIA);
        RuleFor(request => request.Status).NotEmpty().WithMessage(ResourceMessagesException.STATUS_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Status), () =>
        {
            RuleFor(request => request.Status)
                .Must(status => status == "Ativo" || status == "Inativo")
                .WithMessage(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", "Ativo, Inativo"));
        });
    }
}
