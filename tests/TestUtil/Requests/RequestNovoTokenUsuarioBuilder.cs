using Bogus;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestNovoTokenUsuarioBuilder
{
    public static RequestNovoTokenUsuario Build()
    {
        return new Faker<RequestNovoTokenUsuario>()
            .RuleFor(request => request.RefreshToken, faker => faker.Random.AlphaNumericUrl(24));
    }

    public static RequestNovoTokenUsuario Build(RefreshToken refreshToken)
    {
        return new RequestNovoTokenUsuario
        {
            RefreshToken = refreshToken.Valor
        };
    }
}
