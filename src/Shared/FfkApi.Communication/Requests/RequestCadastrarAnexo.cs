using Microsoft.AspNetCore.Http;

namespace FfkApi.Communication.Requests;

public class RequestCadastrarAnexo
{
    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public IFormFile? Arquivo { get; set; }
}