using FfkApi.API.Documentation.OperationFilter;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.Security;

public static class SwaggerSecurityExtension
{
    public static void AddSwaggerSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            const string nomeEsquema = "Bearer";

            options.AddSecurityDefinition(nomeEsquema, new OpenApiSecurityScheme
            {
                Description = @"Cabeçalho de autorização JWT usando o esquema Bearer.
                    Insira 'Bearer' [espaço] e seu token no início do texto abaixo.
                    Exemplo: 'Bearer 123abcde'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = nomeEsquema
            });

            options.AddSecurityDefinition("AppToken", new OpenApiSecurityScheme
            {
                Description = "Token de aplicação (x-app-token) necessário em todas as requisições",
                Name = "x-app-token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "AppToken"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = nomeEsquema
                        },
                        Scheme = "oauth2",
                        Name = nomeEsquema,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "AppToken"
                        },
                        Scheme = "apiKey",
                        Name = "x-app-token",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            var xmlFiles = new[]
            {
                "FfkApi.API.xml",
                "FfkApi.Communication.xml"
            };

            foreach (var xmlFile in xmlFiles)
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
            }

            options.ExampleFilters();
            options.OperationFilter<ConvertExampleToExamplesOperationFilter>();
            options.OperationFilter<ErrorDocsOperationFilter>(configuration);
        });
    }
}
