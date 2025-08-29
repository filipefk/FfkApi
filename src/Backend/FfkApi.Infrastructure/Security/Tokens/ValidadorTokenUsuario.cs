using FfkApi.Domain.Security.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FfkApi.Infrastructure.Security.Tokens;

public class ValidadorTokenUsuario : ChaveSeguraJwt, IValidadorTokenUsuario
{
    private readonly string _chaveAssinatura;

    public ValidadorTokenUsuario(string chaveAssinatura)
    {
        _chaveAssinatura = chaveAssinatura;
    }

    public Guid ValidarEPegarIdUsuario(string token)
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

        var idUsuario = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        return Guid.Parse(idUsuario);
    }

    public Guid PegarIdUsuario(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ReadJwtToken(token);

        var idUsuario = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        return Guid.Parse(idUsuario);
    }
}
