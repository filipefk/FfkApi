using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.SistemaCliente.Cadastrar;

public class CadastrarSistemaClienteValidator : AbstractValidator<RequestCadastrarSistemaCliente>
{
    public CadastrarSistemaClienteValidator()
    {
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
