using FfkApi.Application.UseCases.Anexo.Alterar;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Anexo.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarAnexoValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new AlterarAnexoValidator();

        var request = RequestAlterarAnexoBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Id_Vazio(string? id)
    {
        var validator = new AlterarAnexoValidator();

        var request = RequestAlterarAnexoBuilder.Build();
        request.Id = id;

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
        var validator = new AlterarAnexoValidator();

        var request = RequestAlterarAnexoBuilder.Build();
        request.Id = id;

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
        var validator = new AlterarAnexoValidator();

        var request = RequestAlterarAnexoBuilder.Build();
        request.Nome = nome;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.NOME_VAZIO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Descricao_Vazia(string? descricao)
    {
        var validator = new AlterarAnexoValidator();

        var request = RequestAlterarAnexoBuilder.Build();
        request.Descricao = descricao;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DESCRICAO_VAZIA));
    }
}
