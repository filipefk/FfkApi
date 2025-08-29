using FfkApi.Application.UseCases.Usuario.AlterarPermissoes;
using FfkApi.Domain.Entities;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.Entities;
using TestUtil.InlineData;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.Usuario.AlterarPermissoes;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarPermissoesUsuarioUseCaseTest
{
    [Test]
    public async Task Sucesso_Com_Todos_Campos()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = usuarioAlteracao.Id.ToString();

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, perfisAcesso: perfisAcesso, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public async Task Sucesso_Sem_Perfis_Acesso(List<string>? perfisAcesso)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = usuarioAlteracao.Id.ToString();
        request.PerfisAcesso = perfisAcesso;

        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public async Task Sucesso_Sem_Permissoes(List<string>? permissoes)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = usuarioAlteracao.Id.ToString();
        request.Permissoes = permissoes;

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, perfisAcesso: perfisAcesso);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, perfisAcesso: perfisAcesso, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Nenhuma_Alteracao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build(usuarioAlteracao);

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, perfisAcesso: perfisAcesso, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NENHUMA_ALTERACAO));
    }

    [Test]
    public async Task Erro_Nenhum_Perfil_Acesso_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = usuarioAlteracao.Id.ToString();

        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        var mensagemEsperada = ResourceMessagesException.PERFIS_ACESSO_NAO_ENCONTRADOS.Replace("{lista}", request.PerfisAcesso!.ListaSepadadaPorVirgula());
        Assert.That(mensagensDeErro, Contains.Item(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Algum_Perfil_Acesso_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = usuarioAlteracao.Id.ToString();

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        request.PerfisAcesso!.Add("PerfilAcessoInvalido");

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, perfisAcessoRequest: request.PerfisAcesso, perfisAcesso: perfisAcesso, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        var mensagemEsperada = ResourceMessagesException.PERFIS_ACESSO_NAO_ENCONTRADOS.Replace("{lista}", "PerfilAcessoInvalido");
        Assert.That(mensagensDeErro, Contains.Item(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nenhuma_Permissao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = usuarioAlteracao.Id.ToString();

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, perfisAcesso: perfisAcesso);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        var mensagemEsperada = ResourceMessagesException.PERMISSOES_NAO_ENCONTRADAS.Replace("{lista}", request.Permissoes!.ListaSepadadaPorVirgula());
        Assert.That(mensagensDeErro, Contains.Item(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Alguma_Permissao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = usuarioAlteracao.Id.ToString();

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        request.Permissoes!.Add("PermissaoInvalida");

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, perfisAcesso: perfisAcesso, permissoesRequest: request.Permissoes, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        var mensagemEsperada = ResourceMessagesException.PERMISSOES_NAO_ENCONTRADAS.Replace("{lista}", "PermissaoInvalida");
        Assert.That(mensagensDeErro, Contains.Item(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = id;

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioAlteracao: usuarioAlteracao, perfisAcesso: perfisAcesso, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        var mensagemEsperada = ResourceMessagesException.ID_VAZIO;
        Assert.That(mensagensDeErro, Contains.Item(mensagemEsperada));
    }

    private static AlterarPermissoesUsuarioUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario? usuarioAlteracao = null,
        IList<string>? perfisAcessoRequest = null,
        List<PerfilAcesso>? perfisAcesso = null,
        IList<string>? permissoesRequest = null,
        List<Permissao>? permissoes = null)
    {
        var usuarioRepository = new UsuarioRepositoryBuilder();

        if (usuarioAlteracao != null)
            usuarioRepository.SetupPegarUsuarioPorIdReturnsUsuario(usuarioAlteracao, cancellationToken);

        var perfilAcessoRepository = new PerfilAcessoRepositoryBuilder();
        if (perfisAcesso != null)
        {
            if (perfisAcessoRequest == null)
                perfisAcessoRequest = perfisAcesso.ToListNome()!;
            perfilAcessoRepository.SetupPegarPorNomesAsyncReturnsPerfis(perfisAcessoRequest, perfisAcesso, cancellationToken);
        }

        var permissaoRepository = new PermissaoRepositoryBuilder();
        if (permissoes != null)
        {
            if (permissoesRequest == null)
                permissoesRequest = permissoes.ToListNome()!;
            permissaoRepository.SetupPegarPorNomesAsyncReturnsPermissoes(permissoesRequest, permissoes, cancellationToken);
        }

        return new AlterarPermissoesUsuarioUseCase(
            usuarioRepository.Build(),
            perfilAcessoRepository.Build(),
            permissaoRepository.Build(),
            UnitOfWorkBuilder.Build());
    }
}
