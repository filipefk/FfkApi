using FfkApi.Application.UseCases.Indisponibilidade.Alterar;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Indisponibilidade.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarIndisponibilidadeValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Id_Vazio(string? id)
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
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
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
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
    public void Erro_Descricao_Vazia(string? descricao)
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
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
    public void Erro_Data_Inicial_Vazia(string? dataInicial)
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.DataInicial = dataInicial;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DATA_INICIAL_VAZIA));
    }

    [Test]
    [TestCase("Inválida")]
    [TestCase("31/02/2025")]
    [TestCase("01/15/2025")]
    public void Erro_Data_Inicial_Invalida(string? dataInicial)
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.DataInicial = dataInicial;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DATA_INICIAL_INVALIDA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Data_Final_Vazia(string? dataFinal)
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.DataFinal = dataFinal;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DATA_FINAL_VAZIA));
    }

    [Test]
    [TestCase("Inválida")]
    [TestCase("31/02/2025")]
    [TestCase("01/15/2025")]
    public void Erro_Data_Final_Invalida(string? dataFinal)
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.DataFinal = dataFinal;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DATA_FINAL_INVALIDA));
    }

    [Test]
    public void Erro_Data_Final_Menor_Que_Data_Inicial()
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        var dataInicial = DateOnly.ParseExact(request.DataInicial!, "dd/MM/yyyy");
        request.DataFinal = dataInicial.AddDays(-1).ToString("dd/MM/yyyy");

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DATA_FINAL_MENOR_QUE_DATA_INICIAL));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Email_Usuario_Vazio(string? usuario)
    {
        var validator = new AlterarIndisponibilidadeValidator();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuario;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.EMAIL_USUARIO_VAZIO));
    }
}
