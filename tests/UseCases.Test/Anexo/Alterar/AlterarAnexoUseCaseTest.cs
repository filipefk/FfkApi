using FfkApi.Application.UseCases.Anexo.Alterar;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.Anexo.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarAnexoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarAnexoBuilder.Build();

        var anexoAlteracao = AnexoBuilder.Build();
        anexoAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexoAlteracao: anexoAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Anexo_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarAnexoBuilder.Build();

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ANEXO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Nenhuma_Alteracao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var anexoAlteracao = AnexoBuilder.Build();

        var request = RequestAlterarAnexoBuilder.Build(anexoAlteracao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexoAlteracao: anexoAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NENHUMA_ALTERACAO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Nome_Vazio(string? nome)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarAnexoBuilder.Build();
        request.Nome = nome;

        var anexoAlteracao = AnexoBuilder.Build();
        anexoAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, anexoAlteracao: anexoAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOME_VAZIO));
    }

    private static AlterarAnexoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Anexo? anexoAlteracao = null)
    {
        var anexoRepository = new AnexoRepositoryBuilder();

        if (anexoAlteracao != null)
        {
            anexoRepository.SetupPegarAnexoPorIdReturnsAnexo(anexoAlteracao, cancellationToken);
        }

        return new AlterarAnexoUseCase(
            anexoRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());
    }
}
