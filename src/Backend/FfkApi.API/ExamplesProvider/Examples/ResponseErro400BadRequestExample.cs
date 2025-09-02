using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.ExamplesProvider.Examples;

public class ResponseErro400BadRequestCadastrarEquipeExample : IExamplesProvider<ResponseErro>
{
    public ResponseErro GetExamples()
    {
        var responseErro = new ResponseErro([
            ResourceMessagesException.NOME_VAZIO,
            ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA
        ])
        {
            TokenEstaExpirado = false
        };
        return responseErro;
    }
}

public class ResponseErro400BadRequestAlterarPegarExample : IExamplesProvider<ResponseErro>
{
    public ResponseErro GetExamples()
    {
        var responseErro = new ResponseErro([
            ResourceMessagesException.ID_VAZIO
        ])
        {
            TokenEstaExpirado = false
        };
        return responseErro;
    }
}