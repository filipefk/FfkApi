namespace Integracao.Test.InfraestruturaEmMemoria;

public static class TestServerSingleton
{
    private static readonly Lazy<CustomWebApplicationFactory> _instance =
        new(() =>
        {
            var factory = new CustomWebApplicationFactory();
            factory.Server.PreserveExecutionContext = true;
            return factory;
        });

    public static CustomWebApplicationFactory Instance => _instance.Value;
}
