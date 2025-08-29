using System.Net;

namespace FfkApi.Exceptions.ExceptionsBase;

public class InvalidLoginUsuarioException : ExceptionBase
{
    public InvalidLoginUsuarioException() : base(ResourceMessagesException.EMAIL_OU_SENHA_INVALIDOS) { }

    public override IList<string> PegarMensagensDeErro() => [Message];

    public override HttpStatusCode PegarStatusCode() => HttpStatusCode.Unauthorized;
}
