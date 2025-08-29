using FfkApi.Application.UseCases.Usuario.Excluir;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Usuario.Excluir;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ExcluirUsuarioUseCaseTest
{
    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var usuarioExclusao = UsuarioBuilder.Build();

        var request = new RequestExcluirUsuario()
        {
            Id = usuarioExclusao.Id.ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioExclusao: usuarioExclusao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Usuários"]);

        var usuarioExclusao = UsuarioBuilder.Build();

        var request = new RequestExcluirUsuario()
        {
            Id = usuarioExclusao.Id.ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioExclusao: usuarioExclusao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Usuario_Ja_Excluido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var usuarioExclusao = UsuarioBuilder.Build();
        usuarioExclusao.Status = FfkApi.Domain.Enums.StatusUsuario.Excluido;

        var request = new RequestExcluirUsuario()
        {
            Id = usuarioExclusao.Id.ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioExclusao: usuarioExclusao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_JA_EXCLUIDO));
    }

    [Test]
    public async Task Erro_Auto_Exclusao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = new RequestExcluirUsuario()
        {
            Id = usuarioLogado.Id.ToString()
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioExclusao: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.AUTO_EXCLUSAO));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = new RequestExcluirUsuario()
        {
            Id = id,
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioExclusao: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Usuários"]);

        var request = new RequestExcluirUsuario()
        {
            Id = Guid.NewGuid().ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_NAO_ENCONTRADO));
    }

    private static ExcluirUsuarioUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Usuario? usuarioExclusao = null)
    {
        var repository = new UsuarioRepositoryBuilder();

        if (usuarioExclusao != null) repository.SetupPegarUsuarioPorIdReturnsUsuario(usuarioExclusao, cancellationToken);

        return new ExcluirUsuarioUseCase(
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken),
            repository.Build(),
            UnitOfWorkBuilder.Build());
    }
}