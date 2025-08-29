using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace FfkApi.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        if (exception is ExceptionBase projectException)
        {
            Log.Warning("Erro personalizado do tipo: {ExceptionType}", projectException.GetType().Name);
            HandleProjectException(projectException, context);
            return;
        }

        HandleUnknownException(context);
    }

    private static void HandleProjectException(ExceptionBase exception, ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)exception.PegarStatusCode();
        context.Result = new ObjectResult(new ResponseErro(exception.PegarMensagensDeErro()));
    }

    private static void HandleUnknownException(ExceptionContext context)
    {
        var exception = context.Exception;
        var errorMessage = exception.InnerException != null
            ? $"{exception.Message} - {exception.InnerException.Message}"
            : exception.Message;

        Log.Error("Erro não tratado - {ErrorMessage}", errorMessage);
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new ResponseErro(ResourceMessagesException.ERRO_DESCONHECIDO));
    }
}
