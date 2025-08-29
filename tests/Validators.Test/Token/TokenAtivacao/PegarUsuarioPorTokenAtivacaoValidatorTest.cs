using FfkApi.Application.UseCases.Token.TokenAtivacao;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using NUnit.Framework;

namespace UnidadeValidators.Test.Token.TokenAtivacao;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioPorTokenAtivacaoValidatorTest
{
    [Test]
    public void Sucesso()
    {
        var validator = new PegarUsuarioPorTokenAtivacaoValidator();

        var request = new RequestPegarUsuarioPorTokenAtivacao
        {
            TokenAtivacao = "SoNaoPodeSerVazio"
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Token_Ativacao_Vazio(string? tokenAtivacao)
    {
        var validator = new PegarUsuarioPorTokenAtivacaoValidator();

        var request = new RequestPegarUsuarioPorTokenAtivacao
        {
            TokenAtivacao = tokenAtivacao
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.TOKEN_ATIVACAO_VAZIO));
    }
}
