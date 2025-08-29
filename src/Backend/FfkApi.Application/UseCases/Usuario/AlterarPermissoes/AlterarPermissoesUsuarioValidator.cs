using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Usuario.AlterarPermissoes;

public class AlterarPermissoesUsuarioValidator : AbstractValidator<RequestAlterarPermissoesUsuario>
{
    public AlterarPermissoesUsuarioValidator()
    {
        RuleFor(request => request.Id).NotEmpty().WithMessage(ResourceMessagesException.ID_VAZIO);
        When(request => !string.IsNullOrWhiteSpace(request.Id), () =>
        {
            RuleFor(request => request.Id).Must(IdValidator.IdEstaValido).WithMessage(ResourceMessagesException.ID_INVALIDO);
        });
        When(request => !request.PerfisAcesso!.ListaVazia(), () =>
        {
            RuleFor(request => request.PerfisAcesso)
                .Must(perfis => perfis!.All(p => !string.IsNullOrWhiteSpace(p)))
                .WithMessage(ResourceMessagesException.PERFIL_ACESSO_VAZIO);
        });
        When(request => !request.Permissoes!.ListaVazia(), () =>
        {
            RuleFor(request => request.Permissoes)
                .Must(permissoes => permissoes!.All(p => !string.IsNullOrWhiteSpace(p)))
                .WithMessage(ResourceMessagesException.PERMISSAO_VAZIA);
        });
    }
}
