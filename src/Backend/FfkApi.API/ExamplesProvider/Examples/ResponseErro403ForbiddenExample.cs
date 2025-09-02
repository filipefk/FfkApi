using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.ExamplesProvider.Examples;

public class ResponseErro403ForbiddenExample : IExamplesProvider<ResponseErro>
{
    public ResponseErro GetExamples()
    {
        var responseErro = new ResponseErro([
            ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", "Cadastro de Equipes")
        ])
        {
            TokenEstaExpirado = false
        };
        return responseErro;
    }
}