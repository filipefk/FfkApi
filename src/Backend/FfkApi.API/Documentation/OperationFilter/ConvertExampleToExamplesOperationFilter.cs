using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FfkApi.API.Documentation.OperationFilter;

public class ConvertExampleToExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses == null) return;

        foreach (var response in operation.Responses)
        {
            foreach (var content in response.Value.Content)
            {
                if (content.Value.Example != null)
                {
                    content.Value.Extensions["examples"] = new OpenApiObject
                    {
                        [$"Exemplo {response.Key} - {response.Value.Description ?? "Unknown"}"] = new OpenApiObject
                        {
                            ["summary"] = new OpenApiString($"{response.Key} - {response.Value.Description ?? "Unknown"}"),
                            ["value"] = content.Value.Example
                        }
                    };

                    content.Value.Example = null;
                }
            }
        }
    }
}
