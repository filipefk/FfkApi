using FfkApi.Domain.Security.Tokens;
using FfkApi.Infrastructure.Security.Tokens;

namespace TestUtil.Tokens;

public class GeradorTokenUsuarioBuilder
{
    public static IGeradorTokenUsuario Build()
    {
        return new GeradorTokenUsuario(100, "ChaveDeAssinaturaComNoMinimo32Caracteres");
    }
}
