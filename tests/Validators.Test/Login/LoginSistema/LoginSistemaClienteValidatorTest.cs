using FfkApi.Application.UseCases.Login.LoginSistema;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Login.LoginSistema;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LoginSistemaClienteValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new LoginSistemaClienteValidator();

        var request = RequestLoginSistemaClienteBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_AppId_Vazio(string? appId)
    {
        var validator = new LoginSistemaClienteValidator();

        var request = RequestLoginSistemaClienteBuilder.Build();
        request.AppId = appId;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.APP_ID_VAZIO));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public void Erro_AppId_Invalido(string? appId)
    {
        var validator = new LoginSistemaClienteValidator();

        var request = RequestLoginSistemaClienteBuilder.Build();
        request.AppId = appId;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.APP_ID_INVALIDO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Senha_Vazia(string? senha)
    {
        var validator = new LoginSistemaClienteValidator();

        var request = RequestLoginSistemaClienteBuilder.Build();
        request.Senha = senha;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.SENHA_VAZIA));
    }
}
