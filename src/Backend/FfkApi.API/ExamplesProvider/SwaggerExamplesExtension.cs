using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace FfkApi.API.ExamplesProvider;

public static class SwaggerExamplesExtension
{
    public static void AddAssemblySwaggerExamples(this IServiceCollection services)
    {
        AddSwaggerExamplesListaNamespace(services, "FfkApi.API.ExamplesProvider.Examples");
    }

    private static void AddSwaggerExamplesListaNamespace(IServiceCollection services, string strNamespace)
    {
        AddSwaggerExamplesLista(services, PegarClassesQueContemNamespace(strNamespace));
    }

    private static void AddSwaggerExamplesLista(IServiceCollection services, IEnumerable<Type>? lista)
    {
        if (lista == null || !lista.Any())
            return;

        var assemblies = lista
            .Select(t => t.Assembly)
            .Distinct();

        services.AddSwaggerExamplesFromAssemblies(assemblies.ToArray());
    }

    private static IEnumerable<Type>? PegarClassesQueContemNamespace(string strNamespace)
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && t.Namespace != null && t.Namespace.Contains(strNamespace));
    }


}
