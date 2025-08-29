using FfkApi.Communication.Requests;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Equipe.Cadastrar;

public class CadastrarEquipeValidator : AbstractValidator<RequestCadastrarEquipe>
{
    public CadastrarEquipeValidator()
    {
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Descricao).NotEmpty().WithMessage(ResourceMessagesException.DESCRICAO_VAZIA);
        RuleFor(request => request.Status).NotEmpty().WithMessage(ResourceMessagesException.STATUS_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Status), () =>
        {
            RuleFor(request => request.Status)
                .Must(status => EnumUtil.PegarListaNomesEnum<StatusEquipe>(["Indefinido"]).Contains(status!))
                .WithMessage(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", EnumUtil.PegarNomesEnumSeparadosPorVirgula<StatusEquipe>(["Indefinido"])));
        });
        When(request => request.Membros != null, () =>
        {
            RuleForEach(request => request.Membros)
                .NotNull().WithMessage(ResourceMessagesException.MEMBRO_EQUIPE_NULL)
                .ChildRules(membroEquipe =>
                {
                    membroEquipe.When(membro => membro != null, () =>
                    {
                        membroEquipe.RuleFor(membro => membro.Email)
                            .NotEmpty()
                            .WithMessage(ResourceMessagesException.EMAIL_MEMBRO_EQUIPE_VAZIO);
                        membroEquipe.RuleFor(membro => membro.Lider)
                            .NotNull()
                            .WithMessage(ResourceMessagesException.LIDER_NULL);
                    });
                });
            RuleFor(request => request.Membros)
                .Must(membros =>
                    membros!
                        .Where(membro => membro != null && !string.IsNullOrWhiteSpace(membro.Email))
                        .Count() ==
                    membros!
                        .Where(membro => membro != null && !string.IsNullOrWhiteSpace(membro.Email))
                        .Select(membro => membro!.Email).Distinct().Count())
                .WithMessage(ResourceMessagesException.EMAIL_MEMBRO_EQUIPE_REPETIDOS);
        });
    }
}
