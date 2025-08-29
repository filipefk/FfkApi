using FfkApi.Application.UseCases.Anexo.Pegar;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Anexo.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarArquivoAnexoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();

        var request = new RequestPegarArquivoAnexo
        {
            Id = anexo.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexoBanco: anexo, anexoArquivo: anexo);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);

        Assert.That(response.NomeArquivo, Is.Not.Null);
        Assert.That(response.NomeArquivo, Is.EqualTo(anexo.NomeArquivo));

        Assert.That(response.MimeType, Is.Not.Null);
        Assert.That(response.MimeType, Is.EqualTo(anexo.MimeType));

        Assert.That(response.stream, Is.Not.Null);
    }

    [Test]
    public async Task Erro_Arquivo_Anexo_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();

        var request = new RequestPegarArquivoAnexo
        {
            Id = anexo.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexoBanco: anexo);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ANEXO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Anexo_Nao_Encontrado_No_Banco()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexo = AnexoBuilder.Build();

        var request = new RequestPegarArquivoAnexo
        {
            Id = anexo.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexoArquivo: anexo);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ANEXO_NAO_ENCONTRADO));
    }

    private static PegarArquivoAnexoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Anexo? anexoBanco = null,
        FfkApi.Domain.Entities.Anexo? anexoArquivo = null)
    {
        var anexoRepository = new AnexoRepositoryBuilder();

        if (anexoBanco != null)
        {
            anexoRepository.SetupPegarAnexoPorIdReturnsAnexo(anexoBanco, cancellationToken);
        }

        var armazenadorDeArquivoService = new ArmazenadorDeArquivoServiceBuilder();

        if (anexoArquivo != null)
            armazenadorDeArquivoService.SetupObterAsyncReturnsStream(anexoArquivo.NomeArquivoArmazenamento, cancellationToken);

        return new PegarArquivoAnexoUseCase(
            anexoRepository.Build(),
            armazenadorDeArquivoService.Build());
    }
}
