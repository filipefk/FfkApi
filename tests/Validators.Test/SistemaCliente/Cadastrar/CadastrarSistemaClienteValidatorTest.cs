using FfkApi.Application.UseCases.SistemaCliente.Cadastrar;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.SistemaCliente.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarSistemaClienteValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new CadastrarSistemaClienteValidator();

        var request = RequestCadastrarSistemaClienteBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nome_Vazio(string? nome)
    {
        var validator = new CadastrarSistemaClienteValidator();

        var request = RequestCadastrarSistemaClienteBuilder.Build();
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
        var validator = new CadastrarSistemaClienteValidator();

        var request = RequestCadastrarSistemaClienteBuilder.Build();
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
        var validator = new CadastrarSistemaClienteValidator();

        var request = RequestCadastrarSistemaClienteBuilder.Build();
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
        var validator = new CadastrarSistemaClienteValidator();

        var request = RequestCadastrarSistemaClienteBuilder.Build();
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
        var validator = new CadastrarSistemaClienteValidator();

        var request = RequestCadastrarSistemaClienteBuilder.Build();
        request.Status = status;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", "Ativo, Inativo")));
    }
}
