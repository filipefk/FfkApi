using FfkApi.Application.UseCases.Feed.AdicionarAnexo;
using FfkApi.Domain.Configurations;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Feed.AdicionarAnexo;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AdicionarAnexoFeedUseCaseTest
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

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = (long)(_tamanhoMaximoArquivo * 0.01);

        var feed = FeedBuilder.Build();

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feed.Id.ToString());

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo, feed: feed);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);

        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));
        Assert.That(response.NomeArquivo, Is.EqualTo(request.Arquivo!.FileName));
        Assert.That(response.TamanhoBytes, Is.EqualTo(request.Arquivo.Length));
    }

    [Test]
    public async Task Sucesso_Arquivo_Tamanho_Maximo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = _tamanhoMaximoArquivo;

        var feed = FeedBuilder.Build();

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feed.Id.ToString());

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo, feed: feed);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);

        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));
        Assert.That(response.NomeArquivo, Is.EqualTo(request.Arquivo!.FileName));
        Assert.That(response.TamanhoBytes, Is.EqualTo(request.Arquivo.Length));
    }

    [Test]
    public async Task Erro_Arquivo_Muito_Grande()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = _tamanhoMaximoArquivo + 1;

        var feed = FeedBuilder.Build();

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feed.Id.ToString());

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo, feed: feed);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ARQUIVO_MUITO_GRANDE.Replace("{tamanho-maximo}", ConfiguracaoArquivoAnexo.TamanhoMaximoBytesTexto)));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("      ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = _tamanhoMaximoArquivo;

        var feed = FeedBuilder.Build();

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feed.Id.ToString());
        request.Id = id;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo, feed: feed);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_VAZIO));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = _tamanhoMaximoArquivo;

        var feed = FeedBuilder.Build();

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, feed.Id.ToString());
        request.Id = id;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo, feed: feed);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    [Test]
    public async Task Erro_Feed_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();
        anexo.TamanhoBytes = _tamanhoMaximoArquivo;

        var request = RequestAdicionarAnexoFeedBuilder.Build(anexo, Guid.NewGuid().ToString());

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexo: anexo);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.FEED_NAO_ENCONTRADO));
    }


    private static AdicionarAnexoFeedUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Anexo anexo,
        FfkApi.Domain.Entities.Feed? feed = null)
    {
        var salvarAnexoService = new ArmazenadorDeAnexoServiceBuilder();
        salvarAnexoService.SetupSalvarAsyncReturnsAnexo(anexo, cancellationToken);

        var feedRepository = new FeedRepositoryBuilder();
        if (feed != null)
        {
            feedRepository.SetupPegarFeedPorIdReturnsFeed(feed, cancellationToken);
            feedRepository.SetupExisteFeedComIdReturnsTrue(feed.Id, cancellationToken);
        }

        return new AdicionarAnexoFeedUseCase(
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            salvarAnexoService.Build(),
            feedRepository.Build());
    }
}
