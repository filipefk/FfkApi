using FfkApi.Application.UseCases.Equipe.Excluir;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Equipe.Excluir;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ExcluirEquipeUseCaseTest
{
    [Test]
    public async Task Sucesso_Administrador_Excluindo_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = EquipeBuilder.Build();

        var request = new RequestExcluirEquipe
        {
            Id = equipe.Id.ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipe: equipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Excluindo_Equipe_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = EquipeBuilder.Build();

        var request = new RequestExcluirEquipe
        {
            Id = equipe.Id.ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build(organizacao: equipe.Organizacao);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipe: equipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Nao_Administrador_Excluindo_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = EquipeBuilder.Build();

        var request = new RequestExcluirEquipe
        {
            Id = equipe.Id.ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build();

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipe: equipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EQUIPE_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Equipe_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestExcluirEquipe
        {
            Id = Guid.NewGuid().ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EQUIPE_NAO_ENCONTRADA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = EquipeBuilder.Build();

        var request = new RequestExcluirEquipe
        {
            Id = id
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipe: equipe);

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

        var equipe = EquipeBuilder.Build();

        var request = new RequestExcluirEquipe
        {
            Id = id
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipe: equipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    private static ExcluirEquipeUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Equipe? equipe = null)
    {
        var equipeRepository = new EquipeRepositoryBuilder();

        if (equipe != null)
        {
            equipeRepository.SetupPegarEquipePorIdReturnsEquipe(equipe, cancellationToken);
            equipeRepository.SetupPegarEquipePorIdReturnsEquipe(equipe, equipe.Organizacao.Id, cancellationToken);
        }

        return new ExcluirEquipeUseCase(
            equipeRepository.Build(),
            UnitOfWorkBuilder.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken));
    }
}
