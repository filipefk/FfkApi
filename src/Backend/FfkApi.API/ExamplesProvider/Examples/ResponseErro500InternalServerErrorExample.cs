using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.ExamplesProvider.Examples;

public class ResponseErro500InternalServerErrorExample : IExamplesProvider<ResponseErro>
{
    public ResponseErro GetExamples()
    {
        var responseErro = new ResponseErro([
            ResourceMessagesException.ERRO_DESCONHECIDO
        ])
        {
            TokenEstaExpirado = false
        };
        return responseErro;
    }
}
