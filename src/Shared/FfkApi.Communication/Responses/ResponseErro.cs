namespace FfkApi.Communication.Responses;

public class ResponseErro
{
    public IList<string> MensagensDeErro { get; set; }

    public bool TokenEstaExpirado { get; set; }

    public ResponseErro(IList<string> mensagensDeErro) => MensagensDeErro = mensagensDeErro;

    public ResponseErro(string mensagemDeErro) => MensagensDeErro = [mensagemDeErro];
}
