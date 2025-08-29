namespace FfkApi.Infrastructure.Services.Mensageria;

public class RabbitMqOptions
{
    public string Host { get; set; } = string.Empty;
    public int Porta { get; set; } = 0;
    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string NomeFila { get; set; } = string.Empty;
}
