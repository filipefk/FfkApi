namespace FfkApi.Communication.Requests;

/// <summary>
/// Representa as opções de consulta OData para filtragem, ordenação e paginação de resultados.
/// </summary>
public class RequestODataQueryOptions
{
    /// <summary>
    /// Filtro OData a ser aplicado na consulta.
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Critério de ordenação OData.
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Quantidade máxima de registros a serem retornados.
    /// </summary>
    public int? Top { get; set; }

    /// <summary>
    /// Quantidade de registros a serem ignorados (para paginação).
    /// </summary>
    public int? Skip { get; set; }
}