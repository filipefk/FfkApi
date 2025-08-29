using FfkApi.Application.Services.Anexo;
using FfkApi.Application.UseCases.Anexo.Cadastrar;
using FfkApi.Domain.Configurations;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Anexo.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarAnexoUseCaseTest
{
    private const long _tamanhoMaximoArquivo = 1024;

    [SetUp]
    public void SetUp()
    {
        ConfiguracaoArquivoAnexo.Inicializar(_tamanhoMaximoArquivo);
    }

    [Test]
    public async Task Sucesso_Arquivo_Pequeno()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarAnexoBuilder.Build((int)(_tamanhoMaximoArquivo * 0.01));

        var useCase = CriarUseCase(cancellationToken: cancellationToken, nomeArquivo: request.Arquivo!.Name);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);

        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));
        Assert.That(response.NomeArquivo, Is.EqualTo(request.Arquivo.FileName));
        Assert.That(response.TamanhoBytes, Is.EqualTo(request.Arquivo.Length));
    }

    [Test]
    public async Task Sucesso_Arquivo_Tamanho_Maximo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarAnexoBuilder.Build((int)_tamanhoMaximoArquivo);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, nomeArquivo: request.Arquivo!.Name);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);

        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));
        Assert.That(response.NomeArquivo, Is.EqualTo(request.Arquivo.FileName));
        Assert.That(response.TamanhoBytes, Is.EqualTo(request.Arquivo.Length));
    }

    [Test]
    public async Task Erro_Arquivo_Muito_Grande()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarAnexoBuilder.Build((int)_tamanhoMaximoArquivo + 1);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, nomeArquivo: request.Arquivo!.Name);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ARQUIVO_MUITO_GRANDE.Replace("{tamanho-maximo}", ConfiguracaoArquivoAnexo.TamanhoMaximoBytesTexto)));
    }

    private static CadastrarAnexoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        string? nomeArquivo = null)
    {
        var anexoRepository = new AnexoRepositoryBuilder();

        var armazenadorDeArquivoService = new ArmazenadorDeArquivoServiceBuilder();
        armazenadorDeArquivoService.SetupSalvarAsyncReturnsNomeArquivoArmazenamento(nomeArquivo, cancellationToken);

        var mapper = MapperBuilder.Build();

        var salvarAnexoService = new ArmazenadorDeAnexoService(
            anexoRepository.Build(),
            mapper,
            armazenadorDeArquivoService.Build());

        return new CadastrarAnexoUseCase(
            UnitOfWorkBuilder.Build(),
            mapper,
            salvarAnexoService);
    }
}
