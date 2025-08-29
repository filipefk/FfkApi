using System.Net;

namespace FfkApi.Exceptions.ExceptionsBase;

public class ExpiredSessionException : ExceptionBase
{
    public ExpiredSessionException() : base(ResourceMessagesException.SESSAO_EXPIRADA) { }

    public override IList<string> PegarMensagensDeErro() => [Message];

    public override HttpStatusCode PegarStatusCode() => HttpStatusCode.Unauthorized;
}