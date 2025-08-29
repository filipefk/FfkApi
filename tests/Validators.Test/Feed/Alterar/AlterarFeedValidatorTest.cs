using FfkApi.Application.UseCases.Feed.Alterar;
using FfkApi.Domain.Enums;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Feed.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarFeedValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Id_Vazio(string? id)
    {
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
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
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
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
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
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
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
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
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
        request.Status = status;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.STATUS_VAZIO));
    }

    [Test]
    public void Erro_Status_Invalido()
    {
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
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
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
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
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
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
        var validator = new AlterarFeedValidator();

        var request = RequestAlterarFeedBuilder.Build();
        request.ExpiraEm = dataInvalida;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DATA_EXPIRACAO_INVALIDA));
    }
}
