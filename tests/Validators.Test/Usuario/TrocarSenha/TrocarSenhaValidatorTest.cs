using FfkApi.Application.UseCases.Usuario.TrocarSenha;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Usuario.TrocarSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class TrocarSenhaValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new TrocarSenhaValidator();

        var request = RequestTrocarSenhaBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Senha_Antiga_Vazia(string? senha)
    {
        var validator = new TrocarSenhaValidator();

        var request = RequestTrocarSenhaBuilder.Build();
        request.SenhaAntiga = senha;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors.Select(e => e.ErrorMessage).Distinct().Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.SENHA_VAZIA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nova_Senha_Vazia(string? senha)
    {
        var validator = new TrocarSenhaValidator();

        var request = RequestTrocarSenhaBuilder.Build();
        request.NovaSenha = senha;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors.Select(e => e.ErrorMessage).Distinct().Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.SENHA_VAZIA));
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    public void Erro_Senha_Invalida(int passwordLength)
    {
        var validator = new TrocarSenhaValidator();

        var request = RequestTrocarSenhaBuilder.Build(passwordLength);

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors.Select(e => e.ErrorMessage).Distinct().Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.SENHA_INVALIDA));
    }
}