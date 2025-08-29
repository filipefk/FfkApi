using FfkApi.Application.Extension;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Usuario.Cadastrar;

public class CadastrarUsuarioValidator : AbstractValidator<RequestCadastrarUsuario>
{
    public CadastrarUsuarioValidator()
    {
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
        When(request => request.PerfisAcesso != null && request.PerfisAcesso.Count > 0, () =>
        {
            RuleFor(request => request.PerfisAcesso)
                .Must(perfis => perfis!.All(perfil => !string.IsNullOrWhiteSpace(perfil)))
                .WithMessage(ResourceMessagesException.PERFIL_ACESSO_VAZIO);
        });
        When(request => request.Permissoes != null && request.Permissoes.Count > 0, () =>
        {
            RuleFor(request => request.Permissoes)
                .Must(permissoes => permissoes!.All(permissao => !string.IsNullOrWhiteSpace(permissao)))
                .WithMessage(ResourceMessagesException.PERMISSAO_VAZIA);
        });
    }
}