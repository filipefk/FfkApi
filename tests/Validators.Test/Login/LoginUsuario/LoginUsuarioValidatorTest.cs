using FfkApi.Application.UseCases.Login.LoginUsuario;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Login.LoginUsuario;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LoginUsuarioValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new LoginUsuarioValidator();

        var request = RequestLoginUsuarioBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Email_Vazio(string? email)
    {
        var validator = new LoginUsuarioValidator();

        var request = RequestLoginUsuarioBuilder.Build();
        request.Email = email;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_VAZIO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Senha_Vazia(string? senha)
    {
        var validator = new LoginUsuarioValidator();

        var request = RequestLoginUsuarioBuilder.Build();
        request.Senha = senha;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.SENHA_VAZIA));
    }
}
