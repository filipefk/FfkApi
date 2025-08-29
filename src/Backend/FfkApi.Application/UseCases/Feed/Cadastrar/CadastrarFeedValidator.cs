using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Feed.Cadastrar;

public class CadastrarFeedValidator : AbstractValidator<RequestCadastrarFeed>
{
    public CadastrarFeedValidator()
    {
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Descricao).NotEmpty().WithMessage(ResourceMessagesException.DESCRICAO_VAZIA);
        RuleFor(request => request.Status).NotEmpty().WithMessage(ResourceMessagesException.STATUS_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Status), () =>
        {
            RuleFor(request => request.Status)
                .Must(status => EnumUtil.PegarListaNomesEnum<StatusFeed>(["Indefinido"]).Contains(status!))
                .WithMessage(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", EnumUtil.PegarNomesEnumSeparadosPorVirgula<StatusFeed>(["Indefinido"])));
        });
        When(request => request.VisibilidadeEquipes != null, () =>
        {
            RuleForEach(request => request.VisibilidadeEquipes).NotEmpty().WithMessage(ResourceMessagesException.NOME_EQUIPE_VAZIA);
        });
        When(request => request.VisibilidadeUsuarios != null, () =>
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