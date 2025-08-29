using FfkApi.Infrastructure.DataAccess;
using FfkApi.Initialization.DataInitialization;
using Microsoft.Extensions.DependencyInjection;

namespace FfkApi.Initialization.Extension;

public static class DataInitializationExtension
{
    public static void ApplyDataInitialization(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FfkApiDbContext>();
        DadosIniciais.Cadastrar(dbContext);
        //DadosDeTeste.Cadastrar(dbContext);
    }
}
