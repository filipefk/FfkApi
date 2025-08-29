namespace FfkApi.Infrastructure.Services.Email;

public class EmailSendGridOptions
{
    public string UrlApi { get; set; } = string.Empty;
    public string UrlApiQuota { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RemetenteEmailPadrao { get; set; } = string.Empty;
    public string RemetenteNomePadrao { get; set; } = string.Empty;
}
