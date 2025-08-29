using FfkApi.Domain.Security.Tokens;
using FfkApi.Infrastructure.Security.Tokens;

namespace TestUtil.Tokens;

public class GeradorTokenSistemaClienteBuilder
{
    public static IGeradorTokenSistemaCliente Build()
    {
        return new GeradorTokenSistemaCliente(100, "ChaveDeAssinaturaDiferenteComNoMinimo32Caracteres");
    }
}
