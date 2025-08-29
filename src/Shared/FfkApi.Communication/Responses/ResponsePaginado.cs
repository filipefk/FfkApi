namespace FfkApi.Communication.Responses;

public class ResponsePaginado<T>
{
    public IList<T> Resultados { get; private set; }
    public int PaginaAtual { get; private set; }
    public int TotalDePaginas { get; private set; }
    public int TamanhoDaPagina { get; private set; }
    public long QuantidadeTotal { get; private set; }

    public ResponsePaginado(IList<T> resultados, int paginaAtual, int tamanhoDaPagina, long quantidadeTotal)
    {
        Resultados = resultados;
        PaginaAtual = paginaAtual;
        TamanhoDaPagina = tamanhoDaPagina;
        QuantidadeTotal = quantidadeTotal;
        TotalDePaginas = (int)Math.Ceiling(quantidadeTotal / (double)tamanhoDaPagina);
    }
}
