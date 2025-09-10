using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.Documentation.Examples;

public class ResponseErro404NotFoundExample : IExamplesProvider<ResponseErro>
{
    public ResponseErro GetExamples()
    {
        var responseErro = new ResponseErro([
            ResourceMessagesException.EQUIPE_NAO_ENCONTRADA
        ])
        {
            TokenEstaExpirado = false
        };
        return responseErro;
    }
}