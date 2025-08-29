using FfkApi.Application.UseCases.Feed.Cadastrar;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Feed.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarFeedValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new CadastrarFeedValidator();

        var request = RequestCadastrarFeedBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nome_Vazio(string? nome)
    {
        var validator = new CadastrarFeedValidator();

        var request = RequestCadastrarFeedBuilder.Build();
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
        var validator = new CadastrarFeedValidator();

        var request = RequestCadastrarFeedBuilder.Build();
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
        var validator = new CadastrarFeedValidator();

        var request = RequestCadastrarFeedBuilder.Build();
        request.Status = status;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_VAZIO));
    }

    [Test]
    public void Erro_Status_Invalido()
    {
        var validator = new CadastrarFeedValidator();

        var request = RequestCadastrarFeedBuilder.Build();
        request.Status = "Invalido";

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", EnumUtil.PegarNomesEnumSeparadosPorVirgula<StatusFeed>(["Indefinido"]))));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nome_Equipe_Vazia(string? nomeEquipe)
    {
        var validator = new CadastrarFeedValidator();

        var request = RequestCadastrarFeedBuilder.Build();
        request.VisibilidadeEquipes = [nomeEquipe!];

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.NOME_EQUIPE_VAZIA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Email_Usuario_vazio(string? emailUsuario)
    {
        var validator = new CadastrarFeedValidator();

        var request = RequestCadastrarFeedBuilder.Build();
        request.VisibilidadeUsuarios = [emailUsuario!];

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_USUARIO_VAZIO));
    }

    [Test]
    [TestCase("31/02/2025")]
    [TestCase("2025/01/01")]
    [TestCase("10/15/2025")]
    [TestCase("texto")]
    public void Data_Expiracao_Invalida(string dataInvalida)
    {
        var validator = new CadastrarFeedValidator();

        var request = RequestCadastrarFeedBuilder.Build();
        request.ExpiraEm = dataInvalida;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DATA_EXPIRACAO_INVALIDA));
    }
}
