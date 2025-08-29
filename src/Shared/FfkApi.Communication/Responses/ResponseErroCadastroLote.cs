namespace FfkApi.Communication.Responses;

public class ResponseErroCadastroLote<TRequest>
{
    public required TRequest Request { get; set; }
    public required IList<string> MensagensDeErro { get; set; }
}
