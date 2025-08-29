using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.RateLimiting;

namespace FfkApi.API.Security;

public static class RateLimiterExtension
{
    public static void AddRateLimit(this IServiceCollection services, IConfiguration configuration)
    {
        var globalConfig = new
        {
            PermitLimit = configuration.GetValue<int>("Configuracoes:GlobalRateLimit:LimitePermitido"),
            WindowMinutes = configuration.GetValue<int>("Configuracoes:GlobalRateLimit:IntervaloMinutos"),
            QueueLimit = configuration.GetValue<int>("Configuracoes:GlobalRateLimit:LimiteFila")
        };

        var ipConfig = new
        {
            PermitLimit = configuration.GetValue<int>("Configuracoes:IpRateLimit:LimitePermitido"),
            WindowMinutes = configuration.GetValue<int>("Configuracoes:IpRateLimit:IntervaloMinutos"),
            QueueLimit = configuration.GetValue<int>("Configuracoes:IpRateLimit:LimiteFila")
        };

        services.AddRateLimiter(options =>
        {
            var globalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = globalConfig.PermitLimit,
                    Window = TimeSpan.FromMinutes(globalConfig.WindowMinutes),
                    QueueLimit = globalConfig.QueueLimit
                }));

            var ipLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = ipConfig.PermitLimit,
                    Window = TimeSpan.FromMinutes(ipConfig.WindowMinutes),
                    QueueLimit = ipConfig.QueueLimit
                });
            });

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained([globalLimiter, ipLimiter]);

            options.OnRejected = async (context, token) =>
            {
                var response = new ResponseErro([ResourceMessagesException.LIMITE_REQUISICOES_EXCEDIDO]);

                var result = new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status429TooManyRequests,
                };

                var actionContext = new ActionContext(
                    context.HttpContext,
                    context.HttpContext.GetRouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                );

                await result.ExecuteResultAsync(actionContext);
            };
        });
    }
}
