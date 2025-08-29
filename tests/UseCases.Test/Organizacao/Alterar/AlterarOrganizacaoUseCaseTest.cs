using FfkApi.Application.UseCases.Organizacao.Alterar;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.Organizacao.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarOrganizacaoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarOrganizacaoBuilder.Build();

        var organizacaoAlteracao = OrganizacaoBuilder.Build();
        organizacaoAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacaoAlteracao: organizacaoAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarOrganizacaoBuilder.Build();

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Nome_Organizacao_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarOrganizacaoBuilder.Build();

        var organizacaoAlteracao = OrganizacaoBuilder.Build();
        organizacaoAlteracao.Id = Guid.Parse(request.Id!);

        var organizacaoMesmoNome = OrganizacaoBuilder.Build();
        organizacaoMesmoNome.Nome = request.Nome!;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacaoAlteracao: organizacaoAlteracao, organizacaoMesmoNome: organizacaoMesmoNome);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOME_DE_ORGANIZACAO_JA_EXISTE));
    }

    [Test]
    public async Task Erro_Nenhuma_Alteracao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacaoAlteracao = OrganizacaoBuilder.Build();

        var request = RequestAlterarOrganizacaoBuilder.Build(organizacaoAlteracao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacaoAlteracao: organizacaoAlteracao);

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
    public async Task Erro_Descricao_Vazia(string? descricao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var organizacaoAlteracao = OrganizacaoBuilder.Build();

        var request = RequestAlterarOrganizacaoBuilder.Build(organizacaoAlteracao);
        request.Descricao = descricao;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacaoAlteracao: organizacaoAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.DESCRICAO_VAZIA));
    }

    private static AlterarOrganizacaoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Organizacao? organizacaoAlteracao = null,
        FfkApi.Domain.Entities.Organizacao? organizacaoMesmoNome = null)
    {
        var organizacaoRepository = new OrganizacaoRepositoryBuilder();

        if (organizacaoAlteracao != null)
        {
            organizacaoRepository.SetupPegarOrganizacaoPorIdReturnsOrganizacao(organizacaoAlteracao, cancellationToken);
        }

        if (organizacaoMesmoNome != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(organizacaoMesmoNome.Nome, cancellationToken);
        }

        return new AlterarOrganizacaoUseCase(
            organizacaoRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());
    }
}
