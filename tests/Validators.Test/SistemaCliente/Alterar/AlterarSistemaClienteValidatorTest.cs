using FfkApi.Application.UseCases.SistemaCliente.Alterar;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.SistemaCliente.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarSistemaClienteValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Id_Vazio(string? id)
    {
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
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
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
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
    public void Erro_AppId_Vazio(string? appId)
    {
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
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
    public void Erro_AppId_Invalido(string appId)
    {
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
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
    public void Erro_Nome_Vazio(string? nome)
    {
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
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
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
        request.Descricao = descricao;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DESCRICAO_VAZIA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Senha_Vazia(string? senha)
    {
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
        request.Senha = senha;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.SENHA_VAZIA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Status_Vazio(string? status)
    {
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
        request.Status = status;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_VAZIO));
    }

    [Test]
    [TestCase("Indefinido")]
    [TestCase("Excluido")]
    [TestCase("Ausente")]
    public void Erro_Status_Invalido(string? status)
    {
        var validator = new AlterarSistemaClienteValidator();

        var request = RequestAlterarSistemaClienteBuilder.Build();
        request.Status = status;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", "Ativo, Inativo")));
    }
}
