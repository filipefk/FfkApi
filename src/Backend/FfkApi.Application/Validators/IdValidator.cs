using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.Validators;

public static class IdValidator
{
    public static Guid ValidarId(string? id, string? mensagem = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            if (string.IsNullOrWhiteSpace(mensagem))
                mensagem = ResourceMessagesException.ID_VAZIO;
            throw new ErrorOnValidationException([mensagem]);
        }

        if (!Guid.TryParse(id, out Guid idValido))
        {
            if (string.IsNullOrWhiteSpace(mensagem))
                mensagem = ResourceMessagesException.ID_INVALIDO;
            throw new ErrorOnValidationException([mensagem]);
        }
        return idValido;
    }

    public static bool IdEstaValido(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        if (Guid.TryParse(id, out Guid _))
        {
            return true;
        }
        return false;
    }
}
