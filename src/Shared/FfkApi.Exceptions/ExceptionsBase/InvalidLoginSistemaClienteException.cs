using System.Net;

namespace FfkApi.Exceptions.ExceptionsBase;

public class InvalidLoginSistemaClienteException : ExceptionBase
{
    public InvalidLoginSistemaClienteException() : base(ResourceMessagesException.APP_ID_OU_SENHA_INVALIDOS) { }

    public override IList<string> PegarMensagensDeErro() => [Message];

    public override HttpStatusCode PegarStatusCode() => HttpStatusCode.Unauthorized;
}
