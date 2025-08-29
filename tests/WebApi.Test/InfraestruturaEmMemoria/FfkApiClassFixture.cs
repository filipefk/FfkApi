using Integracao.Test.InfraestruturaEmMemoria.Helpers;

namespace Integracao.Test.InfraestruturaEmMemoria;

public class FfkApiClassFixture
{
    private const string _appToken = "testes-2blHt60aerveQI2UaASZssPntfaB8alE6uJRnQdvbkk";
    protected HttpClient _httpClient;
    protected CustomWebApplicationFactory _factory;
    protected HttpHelper HttpHelper { get; }
    protected CadastroHelper CadastroHelper { get; }

    public FfkApiClassFixture()
    {
        _factory = TestServerSingleton.Instance;
        _httpClient = _factory.CreateClient();
        HttpHelper = new HttpHelper(_httpClient, _appToken);
        CadastroHelper = new CadastroHelper(HttpHelper);
    }
}
