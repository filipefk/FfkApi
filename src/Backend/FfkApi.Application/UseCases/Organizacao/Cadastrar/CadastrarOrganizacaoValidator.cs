using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Organizacao.Cadastrar;

public class CadastrarOrganizacaoValidator : AbstractValidator<RequestCadastrarOrganizacao>
{
    public CadastrarOrganizacaoValidator()
    {
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Descricao).NotEmpty().WithMessage(ResourceMessagesException.DESCRICAO_VAZIA);
    }
}
