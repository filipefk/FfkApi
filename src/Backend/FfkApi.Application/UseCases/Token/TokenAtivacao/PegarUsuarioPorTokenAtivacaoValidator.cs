using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Token.TokenAtivacao;

public class PegarUsuarioPorTokenAtivacaoValidator : AbstractValidator<RequestPegarUsuarioPorTokenAtivacao>
{
    public PegarUsuarioPorTokenAtivacaoValidator()
    {
        RuleFor(request => request.TokenAtivacao).NotEmpty().WithMessage(ResourceMessagesException.TOKEN_ATIVACAO_VAZIO);
    }

}
