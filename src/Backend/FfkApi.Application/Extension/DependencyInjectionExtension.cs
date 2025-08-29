using AutoMapper;
using FfkApi.Application.Services.Anexo;
using FfkApi.Application.Services.AutoMapper;
using FfkApi.Domain.Configurations;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FfkApi.Application.Extension;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddScopedNamespace(services, "FfkApi.Application.UseCases");

        services.AddScoped(_ =>
            new MapperConfiguration(options => { options.AddProfile(new AutoMapping()); }).CreateMapper());

        var tamanhoMaximoBytes = configuration.GetValue<long>("Configuracoes:ArquivoAnexo:TamanhoMaximoBytes");
        ConfiguracaoArquivoAnexo.Inicializar(tamanhoMaximoBytes);

        var urlFront = configuration.GetValue<string>("Configuracoes:Front:Url");
        ConfiguracaoFront.Inicializar(urlFront!);

        services.AddScoped<IArmazenadorDeAnexoService, ArmazenadorDeAnexoService>();
    }

    private static IEnumerable<Type>? PegarClassesQueContemNamespace(string strNamespace)
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass
                && !t.IsAbstract
                && t.IsPublic
                && t.Namespace != null
                && t.Namespace.Contains(strNamespace));
    }

    private static void AddScopedLista(IServiceCollection services, IEnumerable<Type>? lista)
    {
        if (lista == null)
            return;

        foreach (var type in lista)
        {
            var interfaceType = type.GetInterfaces().FirstOrDefault();
            if (interfaceType != null && !(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>)))
            {
                services.AddScoped(interfaceType, type);
            }
        }
    }

    private static void AddScopedNamespace(IServiceCollection services, string strNamespace)
    {
        AddScopedLista(services, PegarClassesQueContemNamespace(strNamespace));
    }
}
