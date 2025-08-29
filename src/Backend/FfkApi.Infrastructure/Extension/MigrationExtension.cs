using FfkApi.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FfkApi.Infrastructure.Extension;

public static class MigrationExtension
{
    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FfkApiDbContext>();
        dbContext.Database.Migrate();
    }
}