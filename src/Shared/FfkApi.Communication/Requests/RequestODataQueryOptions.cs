namespace FfkApi.Communication.Requests;

public class RequestODataQueryOptions
{
    public string? Filter { get; set; }
    public string? OrderBy { get; set; }
    public int? Top { get; set; }
    public int? Skip { get; set; }
}
