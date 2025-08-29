using FfkApi.Infrastructure.Security.Criptografia;

namespace TestUtil.Criptografia;

public static class EncriptadorSenhaBuilder
{
    public static EncriptadorBCrypt Build()
    {
        return new EncriptadorBCrypt();
    }
}