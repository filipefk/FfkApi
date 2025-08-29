using FfkApi.Application.UseCases.Token.RefreshToken;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using NUnit.Framework;

namespace UnidadeValidators.Test.Token.RefreshToken;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class RefreshTokenValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new RefreshTokenValidator();

        var request = new RequestNovoTokenUsuario
        {
            RefreshToken = "SoNaoPodeSerVazio"
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Refresh_Token_Vazio(string? refreshToken)
    {
        var validator = new RefreshTokenValidator();

        var request = new RequestNovoTokenUsuario
        {
            RefreshToken = refreshToken
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.REFRESH_TOKEN_VAZIO));
    }
}
