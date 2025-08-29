using FfkApi.Application.UseCases.Usuario.Pegar;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Usuario.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioPorIdUseCaseTest
{
    [Test]
    public async Task Sucesso_Pegar_Voce_Mesmo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Perfilqualquer"], permissoes: ["Permissao Qualquer"]);

        var useCase = CriarUseCase(cancellationToken, usuarioLogado, usuarioLogado);

        var request = new RequestPegarUsuario()
        {
            Id = usuarioLogado.Id.ToString()
        };

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Id.ToString()));
        Assert.That(response.Id, Is.EqualTo(usuarioLogado.Id));
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(usuarioLogado.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Email));
        Assert.That(response.Email, Is.EqualTo(usuarioLogado.Email));
        Assert.That(!string.IsNullOrWhiteSpace(response.Cpf));
        Assert.That(response.Cpf, Is.EqualTo(usuarioLogado.Cpf));
        Assert.That(!string.IsNullOrWhiteSpace(response.Telefone));
        Assert.That(response.Telefone, Is.EqualTo(usuarioLogado.Telefone));
        Assert.That(!string.IsNullOrWhiteSpace(response.Organizacao));
        Assert.That(response.Organizacao, Is.EqualTo(usuarioLogado.Organizacao.Nome));
        Assert.That(response.PerfisAcesso, Is.Not.Null);
        Assert.That(response.PerfisAcesso, Is.EquivalentTo(usuarioLogado.PerfisAcesso.ToListNome()!));
        Assert.That(response.Permissoes, Is.Not.Null);
        Assert.That(response.Permissoes, Is.EquivalentTo(usuarioLogado.Permissoes.ToListNome()!));
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var usuarioPegar = UsuarioBuilder.Build(perfisAcesso: ["Perfilqualquer"], permissoes: ["Permissao Qualquer"]);

        var useCase = CriarUseCase(cancellationToken, usuarioLogado, usuarioPegar);

        var request = new RequestPegarUsuario()
        {
            Id = usuarioPegar.Id.ToString()
        };

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Id.ToString()));
        Assert.That(response.Id, Is.EqualTo(usuarioPegar.Id));
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(usuarioPegar.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Email));
        Assert.That(response.Email, Is.EqualTo(usuarioPegar.Email));
        Assert.That(!string.IsNullOrWhiteSpace(response.Cpf));
        Assert.That(response.Cpf, Is.EqualTo(usuarioPegar.Cpf));
        Assert.That(!string.IsNullOrWhiteSpace(response.Telefone));
        Assert.That(response.Telefone, Is.EqualTo(usuarioPegar.Telefone));
        Assert.That(!string.IsNullOrWhiteSpace(response.Organizacao));
        Assert.That(response.Organizacao, Is.EqualTo(usuarioPegar.Organizacao.Nome));
        Assert.That(response.PerfisAcesso, Is.Not.Null);
        Assert.That(response.PerfisAcesso, Is.EquivalentTo(usuarioPegar.PerfisAcesso.ToListNome()!));
        Assert.That(response.Permissoes, Is.Not.Null);
        Assert.That(response.Permissoes, Is.EquivalentTo(usuarioPegar.Permissoes.ToListNome()!));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Usuários"]);

        var usuarioPegar = UsuarioBuilder.Build(perfisAcesso: ["Perfilqualquer"], permissoes: ["Permissao Qualquer"]);

        var useCase = CriarUseCase(cancellationToken, usuarioLogado, usuarioPegar);

        var request = new RequestPegarUsuario()
        {
            Id = usuarioPegar.Id.ToString()
        };

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Id.ToString()));
        Assert.That(response.Id, Is.EqualTo(usuarioPegar.Id));
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(usuarioPegar.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Email));
        Assert.That(response.Email, Is.EqualTo(usuarioPegar.Email));
        Assert.That(!string.IsNullOrWhiteSpace(response.Cpf));
        Assert.That(response.Cpf, Is.EqualTo(usuarioPegar.Cpf));
        Assert.That(!string.IsNullOrWhiteSpace(response.Telefone));
        Assert.That(response.Telefone, Is.EqualTo(usuarioPegar.Telefone));
        Assert.That(!string.IsNullOrWhiteSpace(response.Organizacao));
        Assert.That(response.Organizacao, Is.EqualTo(usuarioPegar.Organizacao.Nome));
        Assert.That(response.PerfisAcesso, Is.Not.Null);
        Assert.That(response.PerfisAcesso, Is.EquivalentTo(usuarioPegar.PerfisAcesso.ToListNome()!));
        Assert.That(response.Permissoes, Is.Not.Null);
        Assert.That(response.Permissoes, Is.EquivalentTo(usuarioPegar.Permissoes.ToListNome()!));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var usuarioPegar = UsuarioBuilder.Build();

        var request = new RequestPegarUsuario()
        {
            Id = usuarioPegar.Id.ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioPegar: usuarioPegar);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", "Cadastro de Usuários")));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = new RequestPegarUsuario()
        {
            Id = id,
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioPegar: usuarioLogado);

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

        var request = new RequestPegarUsuario()
        {
            Id = Guid.NewGuid().ToString(),
        };

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioPegar: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<NotFoundException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_NAO_ENCONTRADO));
    }

    private static PegarUsuarioPorIdUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Usuario? usuarioPegar = null)
    {
        var repository = new UsuarioRepositoryBuilder();

        if (usuarioPegar != null) repository.SetupPegarUsuarioPorIdReturnsUsuario(usuarioPegar, cancellationToken);

        return new PegarUsuarioPorIdUseCase(
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken),
            repository.Build(),
            MapperBuilder.Build()
        );
    }
}