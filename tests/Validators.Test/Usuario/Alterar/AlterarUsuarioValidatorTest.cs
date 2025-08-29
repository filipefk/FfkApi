using FfkApi.Application.UseCases.Usuario.Alterar;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Usuario.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarUsuarioValidatorTest
{
    [Test]
    public void Sucesso_Com_Todos_Campos()
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Sucesso_Sem_Telefone(string? telefone)
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Telefone = telefone;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Sucesso_Sem_Organizacao(string? organizacao)
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Organizacao = organizacao;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Id_Vazio(string? id)
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = id!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.ID_VAZIO));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public void Erro_Id_Invalido(string id)
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = id!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.ID_INVALIDO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nome_Vazio(string? nome)
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
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
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Email = email!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_VAZIO));
    }

    [Test]
    public void Erro_Email_Invalido()
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
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
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
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
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Cpf = cpf!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.CPF_INVALIDO));
    }

    [Test]
    [TestCase("letras")]
    [TestCase("(51) 99885-6699")]
    [TestCase("569698745214536985471")]
    public void Erro_Telefone_Invalido(string? telefone)
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Telefone = telefone!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.TELEFONE_INVALIDO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Status_Vazio(string? status)
    {
        var validator = new AlterarUsuarioValidator();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Status = status!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_VAZIO));
    }


}