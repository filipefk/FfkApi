using System.Net;

namespace FfkApi.Exceptions.ExceptionsBase;

public class ForbiddenException : ExceptionBase
{
    public ForbiddenException(string message) : base(message) { }

    public override IList<string> PegarMensagensDeErro() => [Message];

    public override HttpStatusCode PegarStatusCode() => HttpStatusCode.Forbidden;
}
