using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Indisponibilidade.Alterar;

public class AlterarIndisponibilidadeValidator : AbstractValidator<RequestAlterarIndisponibilidade>
{
    public AlterarIndisponibilidadeValidator()
    {
        RuleFor(request => request.Id).NotEmpty().WithMessage(ResourceMessagesException.ID_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Id), () =>
        {
            RuleFor(request => request.Id).Must(IdValidator.IdEstaValido).WithMessage(ResourceMessagesException.ID_INVALIDO);
        });
        RuleFor(request => request.Descricao).NotEmpty().WithMessage(ResourceMessagesException.DESCRICAO_VAZIA);
        RuleFor(request => request.DataInicial).NotEmpty().WithMessage(ResourceMessagesException.DATA_INICIAL_VAZIA);
        When(request => !string.IsNullOrWhiteSpace(request.DataInicial), () =>
        {
            RuleFor(request => request.DataInicial)
                .Must(DataValidator.DataValida)
                .WithMessage(ResourceMessagesException.DATA_INICIAL_INVALIDA);
        });
        RuleFor(request => request.DataFinal).NotEmpty().WithMessage(ResourceMessagesException.DATA_FINAL_VAZIA);
        When(request => !string.IsNullOrWhiteSpace(request.DataFinal), () =>
        {
            RuleFor(request => request.DataFinal)
                .Must(DataValidator.DataValida)
                .WithMessage(ResourceMessagesException.DATA_FINAL_INVALIDA);
        });
        When(request => DataValidator.DataValida(request.DataInicial) &&
            DataValidator.DataValida(request.DataFinal), () =>
            {
                RuleFor(request => request)
                    .Must(request => DataValidator.DataFinalMaiorOuIgualDataInicial(request.DataInicial!, request.DataFinal!))
                    .WithMessage(ResourceMessagesException.DATA_FINAL_MENOR_QUE_DATA_INICIAL);
            });
        RuleFor(request => request.Usuario).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_USUARIO_VAZIO);
    }
}
