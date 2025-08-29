using FfkApi.Domain.Security.Tokens;
using FfkApi.Infrastructure.Security.Tokens;

namespace TestUtil.Tokens;

public class ValidadorTokenUsuarioBuilder
{
    public static IValidadorTokenUsuario Build() => new ValidadorTokenUsuario("ChaveDeAssinaturaComNoMinimo32Caracteres");
}
