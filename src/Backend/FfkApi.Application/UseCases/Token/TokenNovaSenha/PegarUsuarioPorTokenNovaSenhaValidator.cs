using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Token.TokenNovaSenha;

public class PegarUsuarioPorTokenNovaSenhaValidator : AbstractValidator<RequestPegarUsuarioPorTokenNovaSenha>
{
    public PegarUsuarioPorTokenNovaSenhaValidator()
    {
        RuleFor(request => request.TokenNovaSenha).NotEmpty().WithMessage(ResourceMessagesException.TOKEN_NOVA_SENHA_VAZIO);
    }

}
