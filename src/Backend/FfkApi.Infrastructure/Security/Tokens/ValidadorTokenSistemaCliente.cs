using FfkApi.Domain.Security.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FfkApi.Infrastructure.Security.Tokens;

public class ValidadorTokenSistemaCliente : ChaveSeguraJwt, IValidadorTokenSistemaCliente
{
    private readonly string _chaveAssinatura;

    public ValidadorTokenSistemaCliente(string chaveAssinatura)
    {
        _chaveAssinatura = chaveAssinatura;
    }

    public Guid ValidarEPegarIdSistemaCliente(string token)
    {
        var validationParameter = new TokenValidationParameters
        {
            IssuerSigningKey = ChaveSegura(_chaveAssinatura),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, validationParameter, out _);

        var idSistemaCliente = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        return Guid.Parse(idSistemaCliente);
    }

    public Guid PegarIdSistemaCliente(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ReadJwtToken(token);

        var idSistemaCliente = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        return Guid.Parse(idSistemaCliente);
    }
}
