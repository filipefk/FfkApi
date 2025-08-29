using FfkApi.Application.UseCases.Equipe.Cadastrar;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Equipe.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarEquipeValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nome_Vazio(string? nome)
    {
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build();
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
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build();
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
    public void Erro_Status_Vazio(string? status)
    {
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Status = status;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_VAZIO));
    }

    [Test]
    public void Erro_Status_Invalido()
    {
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Status = "Invalido";

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", EnumUtil.PegarNomesEnumSeparadosPorVirgula<StatusEquipe>(["Indefinido"]))));
    }

    [Test]
    public void Erro_Membro_Equipe_Null()
    {
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Membros![0] = null!;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.MEMBRO_EQUIPE_NULL));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Email_Membro_Equipe_Vazio(string? email)
    {
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Membros![0].Email = email;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_MEMBRO_EQUIPE_VAZIO));
    }

    [Test]
    public void Erro_Lider_Membro_Equipe_Null()
    {
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Membros![0].Lider = null;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.LIDER_NULL));
    }

    [Test]
    public void Erro_Email_Membro_Equipe_Repetidos()
    {
        var validator = new CadastrarEquipeValidator();

        var request = RequestCadastrarEquipeBuilder.Build(quantMembros: 3);
        request.Membros![0].Email = request.Membros![1].Email;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_MEMBRO_EQUIPE_REPETIDOS));
    }
}