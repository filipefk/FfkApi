using FfkApi.Application.UseCases.Anexo.Cadastrar;
using FfkApi.Domain.Configurations;
using FfkApi.Exceptions;
using NUnit.Framework;
using TestUtil.Requests;

namespace UnidadeValidators.Test.Anexo.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarAnexoValidatorTest
{
    private const long _tamanhoMaximoArquivo = 1024;

    [SetUp]
    public void SetUp()
    {
        ConfiguracaoArquivoAnexo.Inicializar(_tamanhoMaximoArquivo);
    }

    [Test]
    public void Sucesso_Arquivo_Pequeno()
    {
        var validator = new CadastrarAnexoValidator();

        var request = RequestCadastrarAnexoBuilder.Build((int)(_tamanhoMaximoArquivo * 0.01));

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Sucesso_Arquivo_Tamanho_Maximo()
    {
        var validator = new CadastrarAnexoValidator();

        var request = RequestCadastrarAnexoBuilder.Build((int)_tamanhoMaximoArquivo);

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Erro_Arquivo_Muito_Grande()
    {
        var validator = new CadastrarAnexoValidator();

        var request = RequestCadastrarAnexoBuilder.Build((int)_tamanhoMaximoArquivo + 1);

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.ARQUIVO_MUITO_GRANDE.Replace("{tamanho-maximo}", ConfiguracaoArquivoAnexo.TamanhoMaximoBytesTexto)));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public void Erro_Nome_Vazio(string? nome)
    {
        var validator = new CadastrarAnexoValidator();

        var request = RequestCadastrarAnexoBuilder.Build();
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
        var validator = new CadastrarAnexoValidator();

        var request = RequestCadastrarAnexoBuilder.Build();
        request.Descricao = descricao;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.DESCRICAO_VAZIA));
    }

    [Test]
    public void Erro_Arquivo_Null()
    {
        var validator = new CadastrarAnexoValidator();

        var request = RequestCadastrarAnexoBuilder.Build();
        request.Arquivo = null;

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.ARQUIVO_VAZIO));
    }

    [Test]
    public void Erro_Arquivo_Vazio()
    {
        var validator = new CadastrarAnexoValidator();

        var request = RequestCadastrarAnexoBuilder.Build(0);

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);

        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(ResourceMessagesException.ARQUIVO_VAZIO));
    }
}
