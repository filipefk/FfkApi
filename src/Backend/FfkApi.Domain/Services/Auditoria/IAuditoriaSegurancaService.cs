namespace FfkApi.Domain.Services.Auditoria;

public interface IAuditoriaSegurancaService
{
    Task LogAsync(string evento, string? usuario, string? caminho, string? metodo, string? ip, string? detalhes = null);
}
