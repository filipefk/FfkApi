using FfkApi.Application.UseCases.Organizacao.Cadastrar;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.Organizacao.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarOrganizacaoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoBuilder.Build();

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);
        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));
    }

    [Test]
    public async Task Erro_Nome_Organizacao_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build();
        request.Nome = organizacao.Nome;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOME_DE_ORGANIZACAO_JA_EXISTE));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Descricao_Vazia(string? descricao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarOrganizacaoBuilder.Build();
        request.Descricao = descricao;

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.DESCRICAO_VAZIA));
    }

    private static CadastrarOrganizacaoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Organizacao? organizacao = null)
    {
        var organizacaoRepository = new OrganizacaoRepositoryBuilder();

        if (organizacao != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(organizacao.Nome, cancellationToken);
        }

        return new CadastrarOrganizacaoUseCase(
            organizacaoRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());
    }
}
