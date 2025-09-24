using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.Documentation.Examples;

public class ResponseErro401UnauthorizedExample : IExamplesProvider<ResponseErro>
{
    public ResponseErro GetExamples()
    {
        var responseErro = new ResponseErro([
            ResourceMessagesException.TOKEN_EXPIRADO
        ])
        {
            TokenEstaExpirado = true
        };
        return responseErro;
    }
}
