using FfkApi.Application.UseCases.Usuario.NovaSenha;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Usuario.NovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class NovaSenhaUsuarioValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new NovaSenhaUsuarioValidator();

        var request = RequestNovaSenhaUsuarioBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Senha_Vazia(string? senha)
    {
        var validator = new NovaSenhaUsuarioValidator();

        var request = RequestNovaSenhaUsuarioBuilder.Build();
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
        var validator = new NovaSenhaUsuarioValidator();

        var request = RequestNovaSenhaUsuarioBuilder.Build(passwordLength: passwordLength);

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors.Select(e => e.ErrorMessage).Distinct().Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.SENHA_INVALIDA));
    }
}
