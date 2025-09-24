using FfkApi.API.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FfkApi.API.Documentation.OperationFilter;

public class ErrorDocsOperationFilter : IOperationFilter
{
    private readonly IConfiguration _configuration;

    public ErrorDocsOperationFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var controllerType = context.MethodInfo.DeclaringType;
        if (controllerType == null || controllerType.Name != nameof(EquipeController))
            return;

        var url = _configuration["Configuracoes:UrlsPortalDesenvolvedor:CodigosDeErro"];

        foreach (var response in operation.Responses)
        {
            if (response.Key.StartsWith("4"))
            {
                response.Value.Description += $" Consulte a <a href=\"{url}\" target=\"_blank\">tabela de erros</a>.";
            }
        }
    }
}