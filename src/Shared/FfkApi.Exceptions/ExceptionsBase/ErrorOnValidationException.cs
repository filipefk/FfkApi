using System.Net;

namespace FfkApi.Exceptions.ExceptionsBase;

public class ErrorOnValidationException : ExceptionBase
{
    private readonly IList<string> _mensagensDeErro;

    public ErrorOnValidationException(IList<string> mensagensDeErro) : base(string.Empty)
    {
        _mensagensDeErro = mensagensDeErro;
    }

    public override IList<string> PegarMensagensDeErro() => _mensagensDeErro;

    public override HttpStatusCode PegarStatusCode() => HttpStatusCode.BadRequest;
}
