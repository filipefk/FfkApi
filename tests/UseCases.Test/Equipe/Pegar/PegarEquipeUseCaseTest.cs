using FfkApi.Application.UseCases.Equipe.Pegar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Equipe.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarEquipeUseCaseTest
{
    private static void AssertResponseComEquipe(ResponseDadosEquipe? response, FfkApi.Domain.Entities.Equipe equipe)
    {
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Id, Is.Not.Null);
        Assert.That(response.Id, Is.EqualTo(equipe.Id));
        Assert.That(response.Nome, Is.EqualTo(equipe.Nome));
        Assert.That(response.Descricao, Is.EqualTo(equipe.Descricao));
        Assert.That(response.Organizacao, Is.EqualTo(equipe.Organizacao.Nome));

        var equipeMembrosByEmail = equipe.Membros
            .ToDictionary(m => m.Usuario.Email);
        var responseMembrosByEmail = response.Membros.ToDictionary(m => m.Email);

        Assert.That(responseMembrosByEmail.Keys, Is.EquivalentTo(equipeMembrosByEmail.Keys!));

        foreach (var email in responseMembrosByEmail.Keys)
        {
            var responseMembro = responseMembrosByEmail[email];
            var requestMembro = equipeMembrosByEmail[email];

            Assert.That(responseMembro.Id, Is.Not.Null, $"Id null para o email {email}");
            Assert.That(responseMembro.IdUsuario, Is.Not.Null, $"IdUsuario null para o email {email}");
            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.Nome), $"IdUsuario null para o email {email}");
            Assert.That(responseMembro.Lider, Is.EqualTo(requestMembro.Lider), $"Lider diferente para o email {email}");
        }
    }

    [Test]
    public async Task Sucesso_Administrador_Pegando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = EquipeBuilder.Build();

        var request = new RequestPegarEquipe
        {
            Id = equipe.Id.ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipe: equipe);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComEquipe(response, equipe);
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Pegando_Equipe_Da_Propria_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = EquipeBuilder.Build();

        var request = new RequestPegarEquipe
        {
            Id = equipe.Id.ToString()
        };

        var usuarioLogado = UsuarioBuilder.Build(organizacao: equipe.Organizacao);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipe: equipe);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComEquipe(response, equipe);
    }

    [Test]
    public async Task Erro_Nao_Administrador_Pegando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var equipe = EquipeBuilder.Build();

        var request = new RequestPegarEquipe
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

        var request = new RequestPegarEquipe
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

        var request = new RequestPegarEquipe
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

        var request = new RequestPegarEquipe
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

    private static PegarEquipeUseCase CriarUseCase(
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

        return new PegarEquipeUseCase(
            equipeRepository.Build(),
            MapperBuilder.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken));
    }
}
