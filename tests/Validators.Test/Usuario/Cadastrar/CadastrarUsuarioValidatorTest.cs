using FfkApi.Application.UseCases.Usuario.Cadastrar;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.InlineData;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Usuario.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarUsuarioValidatorTest
{
    [Test]
    public void Sucesso_com_Todos_Campos()
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Sucesso_Sem_Telefone(string telefone)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Telefone = telefone;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public void Sucesso_Perfis_Acesso_Vazio(List<string>? perfisAcesso)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.PerfisAcesso = perfisAcesso;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public void Sucesso_Permissoes_Vazia(List<string>? permissoes)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Permissoes = permissoes;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nome_Vazio(string? nome)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
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
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Email = email!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_VAZIO));
    }

    [Test]
    public void Erro_Email_Invalido()
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
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

    [Test]
    [TestCase("letras")]
    [TestCase("(51) 99885-6699")]
    [TestCase("569698745214536985471")]
    public void Erro_Telefone_Invalido(string? telefone)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
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
    public void Erro_Perfil_Acesso_Vazio(string? valor)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.PerfisAcesso!.Add(valor!);

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.PERFIL_ACESSO_VAZIO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Permissao_Vazia(string? valor)
    {
        var validator = new CadastrarUsuarioValidator();

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Permissoes!.Add(valor!);

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.PERMISSAO_VAZIA));
    }
}