using System.Net;

namespace FfkApi.Exceptions.ExceptionsBase;

public class NotFoundException : ExceptionBase
{
    public NotFoundException(string message) : base(message) { }

    public override IList<string> PegarMensagensDeErro() => [Message];

    public override HttpStatusCode PegarStatusCode() => HttpStatusCode.NotFound;
}
