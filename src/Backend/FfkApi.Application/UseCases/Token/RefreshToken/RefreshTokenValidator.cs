using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Token.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RequestNovoTokenUsuario>
{
    public RefreshTokenValidator()
    {
        RuleFor(request => request.RefreshToken).NotEmpty().WithMessage(ResourceMessagesException.REFRESH_TOKEN_VAZIO);
    }
}
