namespace FfkApi.Communication.Responses;

/// <summary>
/// Representa uma resposta paginada contendo uma lista de resultados e informações de paginação.
/// </summary>
/// <typeparam name="T">Tipo dos itens contidos na lista de resultados.</typeparam>
public class ResponsePaginado<T>
{
    /// <summary>
    /// Lista de resultados da página atual.
    /// </summary>
    public IList<T> Resultados { get; private set; }

    /// <summary>
    /// Número da página atual (base 1).
    /// </summary>
    public int PaginaAtual { get; private set; }

    /// <summary>
    /// Quantidade total de páginas disponíveis.
    /// </summary>
    public int TotalDePaginas { get; private set; }

    /// <summary>
    /// Tamanho da página (quantidade de itens por página).
    /// </summary>
    public int TamanhoDaPagina { get; private set; }

    /// <summary>
    /// Quantidade total de itens disponíveis.
    /// </summary>
    public long QuantidadeTotal { get; private set; }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ResponsePaginado{T}"/> com os resultados e informações de paginação.
    /// </summary>
    /// <param name="resultados">Lista de resultados da página atual.</param>
    /// <param name="paginaAtual">Número da página atual.</param>
    /// <param name="tamanhoDaPagina">Tamanho da página (quantidade de itens por página).</param>
    /// <param name="quantidadeTotal">Quantidade total de itens disponíveis.</param>
    public ResponsePaginado(IList<T> resultados, int paginaAtual, int tamanhoDaPagina, long quantidadeTotal)
    {
        Resultados = resultados;
        PaginaAtual = paginaAtual;
        TamanhoDaPagina = tamanhoDaPagina;
        QuantidadeTotal = quantidadeTotal;
        TotalDePaginas = (int)Math.Ceiling(quantidadeTotal / (double)tamanhoDaPagina);
    }
}