using FfkApi.Application.UseCases.Indisponibilidade.Excluir;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Indisponibilidade.Excluir;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ExcluirIndisponibilidadeUseCaseTest
{
    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var indisponibilidade = IndisponibilidadeBuilder.Build();

        var request = new RequestExcluirIndisponibilidade
        {
            Id = indisponibilidade.Id.ToString()
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeExclusao: indisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Indisponibilidades"]);
        var indisponibilidade = IndisponibilidadeBuilder.Build();

        var request = new RequestExcluirIndisponibilidade
        {
            Id = indisponibilidade.Id.ToString()
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeExclusao: indisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Excluindo_Para_Si_Mesmo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();
        var indisponibilidade = IndisponibilidadeBuilder.Build(usuarioLogado);

        var request = new RequestExcluirIndisponibilidade
        {
            Id = indisponibilidade.Id.ToString()
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeExclusao: indisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Excluindo_Para_Outro_Usuario()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();
        var indisponibilidade = IndisponibilidadeBuilder.Build();

        var request = new RequestExcluirIndisponibilidade
        {
            Id = indisponibilidade.Id.ToString()
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeExclusao: indisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", "Cadastro de Indisponibilidades")));
    }

    [Test]
    public async Task Erro_Indisponibilidade_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var request = new RequestExcluirIndisponibilidade
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

        var request = new RequestExcluirIndisponibilidade
        {
            Id = id
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeExclusao: indisponibilidade);

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

        var request = new RequestExcluirIndisponibilidade
        {
            Id = id
        };

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeExclusao: indisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    private static ExcluirIndisponibilidadeUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Indisponibilidade? indisponibilidadeExclusao = null)
    {
        var indisponibilidadeRepository = new IndisponibilidadeRepositoryBuilder();

        if (indisponibilidadeExclusao != null)
        {
            if (usuarioLogado.TemPerfilAdministrador())
                indisponibilidadeRepository.SetupPegarIndisponibilidadePorIdReturnsIndisponibilidade(
                    indisponibilidadeExclusao,
                    cancellationToken);
            else
                indisponibilidadeRepository.SetupPegarIndisponibilidadePorIdReturnsIndisponibilidade(
                    indisponibilidadeExclusao,
                    usuarioLogado.Organizacao.Id,
                    cancellationToken);
        }

        return new ExcluirIndisponibilidadeUseCase(
            indisponibilidadeRepository.Build(),
            UnitOfWorkBuilder.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken));
    }
}
