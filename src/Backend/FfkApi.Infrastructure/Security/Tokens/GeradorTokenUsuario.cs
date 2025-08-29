using FfkApi.Domain.Security.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FfkApi.Infrastructure.Security.Tokens;

public class GeradorTokenUsuario : ChaveSeguraJwt, IGeradorTokenUsuario
{
    private readonly uint _tempoValidadeMinutos;
    private readonly string _chaveAssinatura;

    public GeradorTokenUsuario(uint tempoValidadeMinutos, string chaveAssinatura)
    {
        _tempoValidadeMinutos = tempoValidadeMinutos;
        _chaveAssinatura = chaveAssinatura;
    }

    public string Gerar(Guid idUsuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, idUsuario.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_tempoValidadeMinutos),
            SigningCredentials = new SigningCredentials(ChaveSegura(_chaveAssinatura), SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

}
