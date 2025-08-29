using FfkApi.Communication.Responses;
using System.Text.Json;

namespace TestUtil.HttpUtil;

public static class HttpResponseUtil
{
    public static async Task<JsonElement.ArrayEnumerator> PegarMensagensDeErro(HttpResponseMessage resposta)
    {
        var dadosDaResposta = await PegarDadosDaResposta(resposta);

        return dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();
    }

    public static async Task<JsonDocument> PegarDadosDaResposta(HttpResponseMessage resposta)
    {
        await using var responseBody = await resposta.Content.ReadAsStreamAsync();

        return await JsonDocument.ParseAsync(responseBody);
    }

    public static async Task<IList<string>> PegarListaMensagensDeErro(HttpResponseMessage resposta)
    {
        var dadosDaResposta = await PegarDadosDaResposta(resposta);
        var lista = dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();

        return lista.Select(e => e.GetString()).ToList()!;
    }

    public static async Task<ResponseErro> ConverterParaResponseErro(HttpResponseMessage resposta)
    {
        var dadosDaResposta = await PegarDadosDaResposta(resposta);
        var root = dadosDaResposta.RootElement;

        var mensagens = root.TryGetProperty("mensagensDeErro", out var mensagensProp)
            ? mensagensProp.EnumerateArray().Select(e => e.GetString()!).ToList()
            : new List<string>();

        var tokenEstaExpirado = root.TryGetProperty("tokenEstaExpirado", out var tokenProp)
            && tokenProp.GetBoolean();

        var responseErro = new ResponseErro(mensagens)
        {
            TokenEstaExpirado = tokenEstaExpirado
        };

        return responseErro;
    }
}
