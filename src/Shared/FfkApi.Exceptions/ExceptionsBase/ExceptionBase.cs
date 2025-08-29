using System.Net;

namespace FfkApi.Exceptions.ExceptionsBase;

public abstract class ExceptionBase : SystemException
{
    public ExceptionBase(string message) : base(message) { }

    public abstract IList<string> PegarMensagensDeErro();
    public abstract HttpStatusCode PegarStatusCode();
}
