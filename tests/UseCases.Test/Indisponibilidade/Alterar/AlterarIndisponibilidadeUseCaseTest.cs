using FfkApi.Application.UseCases.Indisponibilidade.Alterar;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Indisponibilidade.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarIndisponibilidadeUseCaseTest
{
    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build(usuario: usuarioIndisponibilidade);
        indisponibilidadeAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao,
            usuarioIndisponibilidade: usuarioIndisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Indisponibilidades"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build(organizacao: usuarioLogado.Organizacao);

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build(usuario: usuarioIndisponibilidade);
        indisponibilidadeAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao,
            usuarioIndisponibilidade: usuarioIndisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Alterando_Pra_Si_Mesmo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioLogado.Email;

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build(usuario: usuarioLogado);
        indisponibilidadeAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao,
            usuarioIndisponibilidade: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Alterando_Para_Outro_Usuario()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();
        var usuarioIndisponibilidade = UsuarioBuilder.Build();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build(usuario: usuarioIndisponibilidade);
        indisponibilidadeAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao,
            usuarioIndisponibilidade: usuarioIndisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", "Cadastro de Indisponibilidades")));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var request = RequestAlterarIndisponibilidadeBuilder.Build();

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build();
        indisponibilidadeAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Indisponibilidade_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            usuarioIndisponibilidade: usuarioIndisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.INDISPONIBILIDADE_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Existe_Indisponibilidade_Para_Usuario_No_Periodo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build(usuario: usuarioIndisponibilidade);
        indisponibilidadeAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao,
            existeIndisponibilidadeParaUsuarioNoPeriodo: true,
            usuarioIndisponibilidade: usuarioIndisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.JA_EXISTE_INDISPONIBILIDADE_NO_PERIODO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Id_Vazio(string? id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;
        request.Id = id;

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build(usuario: usuarioIndisponibilidade);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao,
            usuarioIndisponibilidade: usuarioIndisponibilidade);

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
        var usuarioIndisponibilidade = UsuarioBuilder.Build();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;
        request.Id = id;

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build(usuario: usuarioIndisponibilidade);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao,
            usuarioIndisponibilidade: usuarioIndisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ID_INVALIDO));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("      ")]
    public async Task Erro_Data_Inicial_Vazia(string? dataInicial)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build();

        var request = RequestAlterarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;
        request.DataInicial = dataInicial;

        var indisponibilidadeAlteracao = IndisponibilidadeBuilder.Build(usuario: usuarioIndisponibilidade);
        indisponibilidadeAlteracao.Id = Guid.Parse(request.Id!);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            indisponibilidadeAlteracao: indisponibilidadeAlteracao,
            usuarioIndisponibilidade: usuarioIndisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.DATA_INICIAL_VAZIA));
    }

    private static AlterarIndisponibilidadeUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Indisponibilidade? indisponibilidadeAlteracao = null,
        bool existeIndisponibilidadeParaUsuarioNoPeriodo = false,
        FfkApi.Domain.Entities.Usuario? usuarioIndisponibilidade = null)
    {
        var indisponibilidadeRepository = new IndisponibilidadeRepositoryBuilder();
        if (indisponibilidadeAlteracao != null)
        {
            if (usuarioLogado.TemPerfilAdministrador())
                indisponibilidadeRepository.SetupPegarIndisponibilidadePorIdReturnsIndisponibilidade(
                    indisponibilidadeAlteracao,
                    cancellationToken);
            else
                indisponibilidadeRepository.SetupPegarIndisponibilidadePorIdReturnsIndisponibilidade(
                    indisponibilidadeAlteracao,
                    usuarioLogado.Organizacao.Id,
                    cancellationToken);
        }

        if (existeIndisponibilidadeParaUsuarioNoPeriodo)
            indisponibilidadeRepository.SetupExisteIndisponibilidadeParaUsuarioNoPeriodoReturnsTrue(cancellationToken);

        var usuarioRepository = new UsuarioRepositoryBuilder();
        if (usuarioIndisponibilidade != null)
        {
            if (usuarioLogado.TemPerfilAdministrador())
                usuarioRepository.SetupPegarUsuarioAptoPorEmailReturnsUsuario(usuarioIndisponibilidade, cancellationToken);
            else
                usuarioRepository.SetupPegarUsuarioAptoPorEmailReturnsUsuario(usuarioIndisponibilidade, usuarioLogado.Organizacao.Id, cancellationToken);
        }

        return new AlterarIndisponibilidadeUseCase(
            indisponibilidadeRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken),
            usuarioRepository.Build());
    }
}
