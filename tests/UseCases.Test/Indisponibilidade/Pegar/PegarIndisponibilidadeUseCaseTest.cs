using FfkApi.Application.UseCases.Indisponibilidade.Pegar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Indisponibilidade.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarIndisponibilidadeUseCaseTest
{
    private static void AssertResponseComIndisponibilidade(ResponseDadosIndisponibilidade response, FfkApi.Domain.Entities.Indisponibilidade indisponibilidade)
    {
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.EqualTo(indisponibilidade.Id));
        Assert.That(response.Descricao, Is.EqualTo(indisponibilidade.Descricao));
        Assert.That(response.DataInicial, Is.EqualTo(indisponibilidade.DataInicial.ToString("dd/MM/yyyy")));
        Assert.That(response.DataFinal, Is.EqualTo(indisponibilidade.DataFinal.ToString("dd/MM/yyyy")));
        Assert.That(response.Usuario.Id, Is.EqualTo(indisponibilidade.Usuario.Id));
        Assert.That(response.Usuario.Nome, Is.EqualTo(indisponibilidade.Usuario.Nome));
        Assert.That(response.Usuario.Email, Is.EqualTo(indisponibilidade.Usuario.Email));
        Assert.That(response.Usuario.Organizacao, Is.EqualTo(indisponibilidade.Usuario.Organizacao.Nome));
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var indisponibilidade = IndisponibilidadeBuilder.Build();

        var request = new RequestPegarIndisponibilidade
        {
            Id = indisponibilidade.Id.ToString()
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadePegar: indisponibilidade);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComIndisponibilidade(response, indisponibilidade);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Indisponibilidades"]);
        var indisponibilidade = IndisponibilidadeBuilder.Build();

        var request = new RequestPegarIndisponibilidade
        {
            Id = indisponibilidade.Id.ToString()
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadePegar: indisponibilidade);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComIndisponibilidade(response, indisponibilidade);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();
        var indisponibilidade = IndisponibilidadeBuilder.Build();

        var request = new RequestPegarIndisponibilidade
        {
            Id = indisponibilidade.Id.ToString()
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadePegar: indisponibilidade);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComIndisponibilidade(response, indisponibilidade);
    }

    [Test]
    public async Task Erro_Indisponibilidade_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var request = new RequestPegarIndisponibilidade
        {
            Id = Guid.NewGuid().ToString()
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.INDISPONIBILIDADE_NAO_ENCONTRADA));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var indisponibilidade = IndisponibilidadeBuilder.Build();

        var request = new RequestPegarIndisponibilidade
        {
            Id = id
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadePegar: indisponibilidade);

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

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var indisponibilidade = IndisponibilidadeBuilder.Build();

        var request = new RequestPegarIndisponibilidade
        {
            Id = id
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadePegar: indisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    private static PegarIndisponibilidadeUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Indisponibilidade? indisponibilidadePegar = null)
    {
        var indisponibilidadeRepository = new IndisponibilidadeRepositoryBuilder();

        if (indisponibilidadePegar != null)
        {
            if (usuarioLogado.TemPerfilAdministrador())
                indisponibilidadeRepository.SetupPegarIndisponibilidadePorIdReturnsIndisponibilidade(
                    indisponibilidadePegar,
                    cancellationToken);
            else
                indisponibilidadeRepository.SetupPegarIndisponibilidadePorIdReturnsIndisponibilidade(
                    indisponibilidadePegar,
                    usuarioLogado.Organizacao.Id,
                    cancellationToken);
        }

        return new PegarIndisponibilidadeUseCase(
            indisponibilidadeRepository.Build(),
            MapperBuilder.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken));
    }
}
