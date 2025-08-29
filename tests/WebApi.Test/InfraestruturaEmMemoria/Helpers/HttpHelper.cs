using FfkApi.Communication.Requests;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Integracao.Test.InfraestruturaEmMemoria.Helpers;

public class HttpHelper
{
    private readonly HttpClient _httpClient;
    private readonly string _appToken;

    public HttpHelper(HttpClient httpClient, string appToken)
    {
        _httpClient = httpClient;
        _appToken = appToken;
    }

    private MultipartFormDataContent FormDataAnexo(RequestCadastrarAnexo request)
    {
        var multipartFormDataContent = new MultipartFormDataContent();

        if (!string.IsNullOrEmpty(request.Nome))
            multipartFormDataContent.Add(new StringContent(request.Nome), nameof(request.Nome));

        if (!string.IsNullOrEmpty(request.Descricao))
            multipartFormDataContent.Add(new StringContent(request.Descricao), nameof(request.Descricao));

        if (request.Arquivo != null)
        {
            var stream = request.Arquivo.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.Arquivo.ContentType ?? "application/octet-stream");
            multipartFormDataContent.Add(fileContent, nameof(request.Arquivo), request.Arquivo.FileName);
        }

        return multipartFormDataContent;
    }

    public async Task<HttpResponseMessage> DoPost(string url, CancellationToken cancellationToken, object request, string token = "", bool addAppToken = true, string? appToken = null)
    {
        LimpaHeaders();

        if (addAppToken)
            AddAppToken(appToken);

        AddAuthorization(token);
        return await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
    }

    public async Task<HttpResponseMessage> DoPostCadastrarAnexo(
        string url,
        CancellationToken cancellationToken,
        object request,
        string token = "",
        bool addAppToken = true,
        string? appToken = null)
    {
        LimpaHeaders();

        if (addAppToken)
            AddAppToken(appToken);

        AddAuthorization(token);

        using var multipartFormDataContent = FormDataAnexo((RequestCadastrarAnexo)request);

        var id = request.GetType().GetProperty("Id")?.GetValue(request)?.ToString() ?? string.Empty;
        if (!string.IsNullOrEmpty(id))
            multipartFormDataContent.Add(new StringContent(id), "Id");

        return await _httpClient.PostAsync(url, multipartFormDataContent, cancellationToken);
    }

    public async Task<HttpResponseMessage> DoGet(string url, CancellationToken cancellationToken, string token = "", bool addAppToken = true, string? appToken = null)
    {
        LimpaHeaders();

        if (addAppToken)
            AddAppToken(appToken);

        AddAuthorization(token);
        return await _httpClient.GetAsync(url, cancellationToken);
    }

    public async Task<HttpResponseMessage> DoDelete(string url, CancellationToken cancellationToken, string token = "", bool addAppToken = true, string? appToken = null)
    {
        LimpaHeaders();

        if (addAppToken)
            AddAppToken(appToken);

        AddAuthorization(token);
        return await _httpClient.DeleteAsync(url, cancellationToken);
    }

    public async Task<HttpResponseMessage> DoPut(string url, CancellationToken cancellationToken, object request, string token = "", bool addAppToken = true, string? appToken = null)
    {
        LimpaHeaders();

        if (addAppToken)
            AddAppToken(appToken);

        AddAuthorization(token);
        return await _httpClient.PutAsJsonAsync(url, request, cancellationToken);
    }

    private void AddAuthorization(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private void AddAppToken(string? appToken)
    {
        appToken ??= _appToken;
        AddHeader("x-app-token", appToken);
    }

    public void AddHeader(string key, string value)
    {
        if (_httpClient.DefaultRequestHeaders.Contains(key))
            _httpClient.DefaultRequestHeaders.Remove(key);

        _httpClient.DefaultRequestHeaders.Add(key, value);
    }

    public void LimpaHeaders()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        if (_httpClient.DefaultRequestHeaders.Contains("x-app-token"))
            _httpClient.DefaultRequestHeaders.Remove("x-app-token");
    }
}
