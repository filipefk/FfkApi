using FfkApi.Application.UseCases.Token.TokenNovaSenha;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using NUnit.Framework;

namespace UnidadeValidators.Test.Token.TokenNovaSenha;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioPorTokenNovaSenhaValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new PegarUsuarioPorTokenNovaSenhaValidator();

        var request = new RequestPegarUsuarioPorTokenNovaSenha
        {
            TokenNovaSenha = "SoNaoPodeSerVazio"
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Token_Nova_Senha_Vazio(string? tokenNovaSenha)
    {
        var validator = new PegarUsuarioPorTokenNovaSenhaValidator();

        var request = new RequestPegarUsuarioPorTokenNovaSenha
        {
            TokenNovaSenha = tokenNovaSenha
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.TOKEN_NOVA_SENHA_VAZIO));
    }
}
