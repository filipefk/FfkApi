using FfkApi.Application.UseCases.Token.TokenAtivacao;
using FfkApi.Communication.Requests;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.Repositories;

namespace UnidadeUseCases.Test.Token.TokenAtivacao;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class RenovarTokenAtivacaoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();
        usuario.Status = FfkApi.Domain.Enums.StatusUsuario.Inativo;

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var request = new RequestRenovarTokenAtivacao
        {
            IdUsuario = usuario.Id.ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuario: usuario, tokenAtivacao: tokenAtivacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();
        usuario.Status = FfkApi.Domain.Enums.StatusUsuario.Inativo;

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var request = new RequestRenovarTokenAtivacao
        {
            IdUsuario = id,
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuario: usuario, tokenAtivacao: tokenAtivacao);

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

        var usuario = UsuarioBuilder.Build();
        usuario.Status = FfkApi.Domain.Enums.StatusUsuario.Inativo;

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var request = new RequestRenovarTokenAtivacao
        {
            IdUsuario = Guid.NewGuid().ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuario: usuario, tokenAtivacao: tokenAtivacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Somente_Usuarios_Inativos()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();
        usuario.Status = FfkApi.Domain.Enums.StatusUsuario.Ativo;

        var tokenAtivacao = TokenAtivacaoBuilder.Build(usuario);

        var request = new RequestRenovarTokenAtivacao
        {
            IdUsuario = usuario.Id.ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuario: usuario, tokenAtivacao: tokenAtivacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAIL_ATIVACAO_SOMENTE_USUARIOS_INATIVOS));
    }

    [Test]
    public async Task Erro_Token_Ativacao_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuario = UsuarioBuilder.Build();
        usuario.Status = FfkApi.Domain.Enums.StatusUsuario.Inativo;

        var request = new RequestRenovarTokenAtivacao
        {
            IdUsuario = usuario.Id.ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuario: usuario);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.TOKEN_ATIVACAO_NAO_ENCONTRADO));
    }

    private static RenovarTokenAtivacaoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario? usuario = null,
        FfkApi.Domain.Entities.TokenAtivacao? tokenAtivacao = null)
    {
        var usuarioRepository = new UsuarioRepositoryBuilder();
        var tokenAtivacaoRepository = new TokenAtivacaoRepositoryBuilder();

        if (usuario != null)
        {
            usuarioRepository.SetupPegarUsuarioPorIdReturnsUsuario(usuario, cancellationToken);
            if (tokenAtivacao != null)
                tokenAtivacaoRepository.SetupPegarTokenAtivacaoPorUsuarioReturnsTokenAtivacao(usuario.Id, tokenAtivacao, cancellationToken);
        }

        return new RenovarTokenAtivacaoUseCase(
            tokenAtivacaoRepository.Build(),
            usuarioRepository.Build(),
            UnitOfWorkBuilder.Build());

    }
}
