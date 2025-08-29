using FfkApi.Application.UseCases.Token.TokenNovaSenha;
using FfkApi.Application.UseCases.Usuario.Cadastrar;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Token.TokenNovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class NovoTokenNovaSenhaValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new NovoTokenNovaSenhaValidator();

        var request = RequestNovoTokenNovaSenhaBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nome_Vazio(string? nome)
    {
        var validator = new NovoTokenNovaSenhaValidator();

        var request = RequestNovoTokenNovaSenhaBuilder.Build();
        request.Nome = nome!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.NOME_VAZIO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Email_Vazio(string? email)
    {
        var validator = new NovoTokenNovaSenhaValidator();

        var request = RequestNovoTokenNovaSenhaBuilder.Build();
        request.Email = email!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_VAZIO));
    }

    [Test]
    public void Erro_Email_Invalido()
    {
        var validator = new NovoTokenNovaSenhaValidator();

        var request = RequestNovoTokenNovaSenhaBuilder.Build();
        request.Email = "AlgumLixoAqui";

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_INVALIDO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Cpf_Vazio(string? cpf)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Cpf = cpf!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.CPF_VAZIO));
    }

    [Test]
    [TestCase("letras")]
    [TestCase("009.304.160-82")]
    [TestCase("11111111111")]
    [TestCase("22222222222")]
    [TestCase("33333333333")]
    [TestCase("44444444444")]
    [TestCase("55555555555")]
    [TestCase("66666666666")]
    [TestCase("77777777777")]
    [TestCase("88888888888")]
    [TestCase("99999999999")]
    [TestCase("00000000000")]
    [TestCase("12345678901")]
    public void Erro_Cpf_Invalido(string? cpf)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Cpf = cpf!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.CPF_INVALIDO));
    }
}
