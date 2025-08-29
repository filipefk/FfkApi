using FfkApi.Application.UseCases.Usuario.AlterarPermissoes;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.InlineData;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Usuario.AlterarPermissoes;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarPermissoesUsuarioValidatorTest
{
    [Test]
    public void Sucesso_Com_Todos_Campos()
    {
        var validator = new AlterarPermissoesUsuarioValidator();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public void Sucesso_Perfis_Acesso_Vazio(List<string>? perfisAcesso)
    {
        var validator = new AlterarPermissoesUsuarioValidator();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.PerfisAcesso = perfisAcesso;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public void Sucesso_Permissoes_Vazia(List<string>? permissoes)
    {
        var validator = new AlterarPermissoesUsuarioValidator();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Permissoes = permissoes;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Id_Vazio(string? id)
    {
        var validator = new AlterarPermissoesUsuarioValidator();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
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
        var validator = new AlterarPermissoesUsuarioValidator();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
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
    public void Erro_Perfil_Acesso_Vazio(string? valor)
    {
        var validator = new AlterarPermissoesUsuarioValidator();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
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
        var validator = new AlterarPermissoesUsuarioValidator();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Permissoes!.Add(valor!);

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.PERMISSAO_VAZIA));
    }
}
