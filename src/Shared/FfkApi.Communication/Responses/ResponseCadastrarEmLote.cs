namespace FfkApi.Communication.Responses;

public class ResponseCadastrarEmLote<TRequest, TResponse>
{
    public IList<TResponse> Cadastrados { get; set; } = [];
    public IList<ResponseErroCadastroLote<TRequest>> Erros { get; set; } = [];
    public int TotalCadastrados => Cadastrados.Count;
    public int TotalErros => Erros.Count;
    public string Resultado
    {
        get
        {
            if (Cadastrados.Count == 0 && Erros.Count == 0)
                return "Indefinido";
            if (Cadastrados.Count > 0 && Erros.Count == 0)
                return "SucessoTotal";
            if (Cadastrados.Count > 0 && Erros.Count > 0)
                return "SucessoParcial";
            return "Falha";
        }
    }
}
