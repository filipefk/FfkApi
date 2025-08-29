using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FfkApi.Infrastructure.Security.Tokens;

public class ChaveSeguraJwt
{
    protected static SymmetricSecurityKey ChaveSegura(string chaveAssinatura)
    {
        var bytes = Encoding.UTF8.GetBytes(chaveAssinatura);

        return new SymmetricSecurityKey(bytes);
    }
}
