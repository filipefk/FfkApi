using FfkApi.Domain.Services.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly.Timeout;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FfkApi.Infrastructure.Services.Email;

public class EnviarEmailSendGridService : IEnviarEmailService
{
    private readonly EmailSendGridOptions _options;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _httpRetryPolicy;
    private readonly AsyncTimeoutPolicy _timeoutPolicy;
    private readonly ILogger<EnviarEmailSendGridService> _logger;
    private static readonly HttpClient _httpClient = new();

    public EnviarEmailSendGridService(
        IOptions<EmailSendGridOptions> options,
        AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy,
        AsyncTimeoutPolicy timeoutPolicy,
        ILogger<EnviarEmailSendGridService> logger)
    {
        _options = options.Value;
        _httpRetryPolicy = httpRetryPolicy;
        _timeoutPolicy = timeoutPolicy;
        _logger = logger;
    }

    public async Task<RespostaEnvioEmail> EnviarEmailAsync(
        string? remetenteEmail,
        string? remetenteNome,
        string destinatarioEmail,
        string destinatarioNome,
        string assunto,
        string? modeloEmail,
        string? textoEmail,
        Dictionary<string, string> variaveis,
        CancellationToken cancellationToken)
    {
        var from = new RequestNomeEmailSendGrid
        {
            Email = remetenteEmail ?? _options.RemetenteEmailPadrao,
            Name = remetenteNome ?? _options.RemetenteNomePadrao,
        };
        var to = new List<RequestNomeEmailSendGrid>(1)
        {
            new RequestNomeEmailSendGrid
            {
                Email = destinatarioEmail,
                Name = destinatarioNome,
            }
        };

        object requestBody;
        if (!string.IsNullOrEmpty(modeloEmail))
        {
            var requestSendGridComTemplate = new RequestSendGridComTemplate
            {
                From = from,
                To = to,
                Subject = assunto,
                Template_id = modeloEmail,
            };

            if (variaveis != null && variaveis.Count > 0)
            {
                requestSendGridComTemplate.Personalization = new List<RequestSendGridPersonalization>(1)
                {
                    new RequestSendGridPersonalization
                    {
                        Email = destinatarioEmail,
                        Data = variaveis
                    }
                };
            }

            requestBody = requestSendGridComTemplate;
        }
        else
        {
            var requestSendGridSemTemplate = new RequestSendGridSemTemplate
            {
                From = from,
                To = to,
                Subject = assunto,
                Text = textoEmail ?? string.Empty,
            };
            requestBody = requestSendGridSemTemplate;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);

        HttpResponseMessage response = await _timeoutPolicy.WrapAsync(_httpRetryPolicy)
            .ExecuteAsync(async ct =>
                await _httpClient.PostAsJsonAsync(_options.UrlApi, requestBody, ct), cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            return new RespostaEnvioEmail
            {
                Enviado = true
            };

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogWarning("Falha ao enviar Email SendGrid. Resposta = '{responseBody}'", responseBody);

        return new RespostaEnvioEmail
        {
            Enviado = false,
            Mensagem = responseBody
        };
    }

    public bool EstaDisponivel()
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);

        try
        {
            var response = _timeoutPolicy.WrapAsync(_httpRetryPolicy)
                .ExecuteAsync(ct => _httpClient.GetAsync(_options.UrlApiQuota, ct), CancellationToken.None)
                .GetAwaiter().GetResult();
            var disponivel = response.StatusCode == System.Net.HttpStatusCode.OK;
            if (!disponivel)
                _logger.LogWarning("Email SendGrid indisponível. StatusCode = {StatusCode}", response.StatusCode);
            return disponivel;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Email SendGrid indisponível");
            return false;
        }
    }

    private class RequestSendGrid
    {
        public RequestNomeEmailSendGrid From { get; set; } = new();
        public IList<RequestNomeEmailSendGrid> To { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
        public IList<RequestSendGridPersonalization>? Personalization { get; set; } = null;
    }

    private class RequestSendGridComTemplate : RequestSendGrid
    {
        public string? Template_id { get; set; } = null;
    }

    private class RequestSendGridSemTemplate : RequestSendGrid
    {
        public string? Text { get; set; } = null;
    }

    private class RequestNomeEmailSendGrid
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private class RequestSendGridPersonalization
    {
        public string Email { get; set; } = string.Empty;
        public object Data { get; set; } = default!;
    }
}