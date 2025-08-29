using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;

namespace UnidadeValidators.Test.IdValidator;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class IdValidatorTest
{
    [Test]
    public void Sucesso_Validar_Id()
    {
        static Guid func() => FfkApi.Application.Validators.IdValidator.ValidarId(Guid.NewGuid().ToString());
        Assert.DoesNotThrow(() => func());
    }

    [Test]
    public void Sucesso_Id_Esta_Valido()
    {
        var estaValido = FfkApi.Application.Validators.IdValidator.IdEstaValido(Guid.NewGuid().ToString());
        Assert.That(estaValido);
    }

    [Test]
    [TestCase("")]
    [TestCase("sdfgsdfg")]
    [TestCase("9f9ae693-wif9-4d6d-jk66-930ecceb7157")]
    [TestCase("        ")]
    public void Sucesso_Id_Nao_Esta_Valido(string id)
    {
        var estaValido = FfkApi.Application.Validators.IdValidator.IdEstaValido(id);
        Assert.That(!estaValido);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Id_Vazio(string id)
    {
        Guid func() => FfkApi.Application.Validators.IdValidator.ValidarId(id);

        var exception = Assert.Throws<ErrorOnValidationException>(() => func());
        Assert.That(exception, Is.Not.Null);

        var mensagensDeErro = exception!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro, Is.Not.Null);

        Assert.That(mensagensDeErro, Has.Count.EqualTo(1));
        Assert.That(mensagensDeErro[0], Is.EqualTo(ResourceMessagesException.ID_VAZIO));
    }

    [Test]
    [TestCase("1234")]
    [TestCase("sdfgsdfg")]
    [TestCase("9f9ae693-wif9-4d6d-jk66-930ecceb7157")]
    public void Erro_Id_Invalido(string id)
    {
        Guid func() => FfkApi.Application.Validators.IdValidator.ValidarId(id);

        var exception = Assert.Throws<ErrorOnValidationException>(() => func());
        Assert.That(exception, Is.Not.Null);

        var mensagensDeErro = exception!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro, Is.Not.Null);

        Assert.That(mensagensDeErro, Has.Count.EqualTo(1));
        Assert.That(mensagensDeErro[0], Is.EqualTo(ResourceMessagesException.ID_INVALIDO));
    }

    [Test]
    [TestCase("", "O Id está vazio")]
    [TestCase("sdfgsdfg", "Este Id não serve")]
    [TestCase("9f9ae693-wif9-4d6d-jk66-930ecceb7157", "Este Id é estranho")]
    [TestCase("        ", "Só espaços não pode")]
    public void Erro_Id_Invalido_Outra_Mensagem(string id, string mensagem)
    {
        Guid func() => FfkApi.Application.Validators.IdValidator.ValidarId(id, mensagem);

        var exception = Assert.Throws<ErrorOnValidationException>(() => func());
        Assert.That(exception, Is.Not.Null);

        var mensagensDeErro = exception!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro, Is.Not.Null);

        Assert.That(mensagensDeErro, Has.Count.EqualTo(1));
        Assert.That(mensagensDeErro[0], Is.EqualTo(mensagem));
    }

}
