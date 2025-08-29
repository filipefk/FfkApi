using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Feed.Alterar;

public class AlterarFeedValidator : AbstractValidator<RequestAlterarFeed>
{
    public AlterarFeedValidator()
    {
        RuleFor(request => request.Id).NotEmpty().WithMessage(ResourceMessagesException.ID_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Id), () =>
        {
            RuleFor(request => request.Id).Must(IdValidator.IdEstaValido).WithMessage(ResourceMessagesException.ID_INVALIDO);
        });
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Descricao).NotEmpty().WithMessage(ResourceMessagesException.DESCRICAO_VAZIA);
        RuleFor(request => request.Status).NotEmpty().WithMessage(ResourceMessagesException.STATUS_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Status), () =>
        {
            RuleFor(request => request.Status)
                .Must(status => EnumUtil.PegarListaNomesEnum<StatusFeed>(["Indefinido"]).Contains(status!))
                .WithMessage(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", EnumUtil.PegarNomesEnumSeparadosPorVirgula<StatusFeed>(["Indefinido"])));
        });
        When(request => request.VisibilidadeEquipes != null && request.VisibilidadeEquipes.Count > 0, () =>
        {
            RuleForEach(request => request.VisibilidadeEquipes).NotEmpty().WithMessage(ResourceMessagesException.NOME_EQUIPE_VAZIA);
        });
        When(request => request.VisibilidadeUsuarios != null && request.VisibilidadeUsuarios.Count > 0, () =>
        {
            RuleForEach(request => request.VisibilidadeUsuarios).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_USUARIO_VAZIO);
        });
        When(request => !string.IsNullOrWhiteSpace(request.ExpiraEm), () =>
        {
            RuleFor(request => request.ExpiraEm)
                .Must(DataValidator.DataValida)
                .WithMessage(ResourceMessagesException.DATA_EXPIRACAO_INVALIDA);
        });
    }
}