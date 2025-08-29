namespace FfkApi.Communication.Responses;

public class ResponseLoginUsuario
{
    public string Nome { get; set; } = string.Empty;
    public ResponseTokens Tokens { get; set; } = default!;
}
