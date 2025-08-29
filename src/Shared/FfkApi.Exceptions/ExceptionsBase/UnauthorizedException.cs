using System.Net;

namespace FfkApi.Exceptions.ExceptionsBase;

public class UnauthorizedException : ExceptionBase
{
    public UnauthorizedException(string message) : base(message) { }

    public override IList<string> PegarMensagensDeErro() => [Message];

    public override HttpStatusCode PegarStatusCode() => HttpStatusCode.Unauthorized;
}
