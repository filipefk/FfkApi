using FfkApi.Application.UseCases.Usuario.Alterar;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.InlineData;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Usuario.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarUsuarioUseCaseTest
{
    [Test]
    [TestCase("Ativo", "")]
    [TestCase("Ausente", null)]
    [TestCase("Ativo", "   ")]
    [TestCase("Ausente", "usuario")]
    public async Task Sucesso_Alterar_Seus_Dados(string status, string? nomeOrganizacao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = usuarioLogado.Id.ToString();
        request.Organizacao = nomeOrganizacao == "usuario" ? usuarioLogado.Organizacao.Nome : nomeOrganizacao;
        request.Status = status;

        var organizacao = OrganizacaoBuilder.Build(usuarioLogado.Organizacao.Nome);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioLogado, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test, TestCaseSource(typeof(StatusAoAlterarStatusUsuarioInlineData), nameof(StatusAoAlterarStatusUsuarioInlineData.ListaPermitidaAlterarStatusDeOutroUsuario))]
    public async Task Sucesso_Administrador(string status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build(usuarioAlteracao);
        request.Status = status;

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioAlteracao, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test, TestCaseSource(typeof(StatusAoAlterarStatusUsuarioInlineData), nameof(StatusAoAlterarStatusUsuarioInlineData.ListaPermitidaAlterarStatusDeOutroUsuario))]
    public async Task Sucesso_Usuario_Com_Permissao(string status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Usuários"]);

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build(usuarioAlteracao);
        request.Status = status;

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioAlteracao, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Alterar_Sua_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build(usuarioLogado);
        request.Organizacao = usuarioLogado.Organizacao.Nome == "Nova" ? "FfkApi" : "Nova";

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioLogado, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ForbiddenException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.SEM_PERMISSAO_ALTERAR_ORGANIZACAO));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build(usuarioAlteracao);

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioAlteracao, organizacao: organizacao);

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

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Usuários"]);

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = id;

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioLogado, organizacao: organizacao);

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

        var request = RequestAlterarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Email_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build(uniqueSuffix: "fake");
        request.Id = usuarioLogado.Id.ToString();
        request.Organizacao = usuarioLogado.Organizacao.Nome;
        request.Status = "Ativo";

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioLogado, email: request.Email, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAIL_JA_EXISTE));
    }

    [Test]
    public async Task Erro_Cpf_Ja_Existe_Na_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = usuarioLogado.Id.ToString();
        request.Organizacao = usuarioLogado.Organizacao.Nome;
        request.Status = "Ativo";

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioLogado, cpf: request.Cpf, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.CPF_JA_EXISTE));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Cpf_Vazio(string? cpf)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = usuarioLogado.Id.ToString();
        request.Organizacao = usuarioLogado.Organizacao.Nome;
        request.Status = "Ativo";
        request.Cpf = cpf;

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioLogado, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.CPF_VAZIO));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = usuarioLogado.Id.ToString();
        request.Status = "Ativo";

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test, TestCaseSource(typeof(StatusAoAlterarStatusUsuarioInlineData), nameof(StatusAoAlterarStatusUsuarioInlineData.ListaInvalidaAlterarStatusDeOutroUsuario))]
    public async Task Erro_Alterar_Status_Invalido_Outro_Usuario(string? status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var usuarioAlteracao = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = usuarioAlteracao.Id.ToString();
        request.Status = status!;

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioAlteracao, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        var listaValida = FfkApi.Domain.Entities.Usuario.StatusPermitidosAoAlterarStatusDeOutroUsuario().ListaSepadadaPorVirgula();
        var mensagemEsperada = ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", listaValida);
        Assert.That(mensagensDeErro, Contains.Item(mensagemEsperada));
    }

    [Test]
    [TestCase("Suspenso")]
    [TestCase("Inativo")]
    [TestCase("StatusInvalido")]
    public async Task Erro_Alterar_Status_Invalido_Seu_Usuario(string? status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = usuarioLogado.Id.ToString();
        request.Organizacao = usuarioLogado.Organizacao.Nome;
        request.Status = status!;

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioAlteracao: usuarioLogado, organizacao: organizacao);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        var listaValida = FfkApi.Domain.Entities.Usuario.StatusPermitidosAoAlterarSeuProprioStatus().ListaSepadadaPorVirgula();
        var mensagemEsperada = ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", listaValida);
        Assert.That(mensagensDeErro, Contains.Item(mensagemEsperada));
    }

    private static AlterarUsuarioUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Usuario? usuarioAlteracao = null,
        string? email = null,
        string? cpf = null,
        FfkApi.Domain.Entities.Organizacao? organizacao = null)
    {
        var usuarioRepository = new UsuarioRepositoryBuilder();

        if (usuarioAlteracao != null)
            usuarioRepository.SetupPegarUsuarioPorIdReturnsUsuario(usuarioAlteracao, cancellationToken);

        if (email != null)
            usuarioRepository.SetupExisteUsuarioComEmailReturnsTrue(email, cancellationToken);

        if (cpf != null)
            usuarioRepository.SetupExisteUsuarioComCpfReturnsTrue(cpf, organizacao?.Id, cancellationToken);

        var organizacaoRepository = new OrganizacaoRepositoryBuilder();

        if (organizacao != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(organizacao.Nome, cancellationToken);
            organizacaoRepository.SetupPegarOrganizacaoPorNomeReturnsOrganizacao(organizacao, cancellationToken);
        }

        return new AlterarUsuarioUseCase(
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken),
            usuarioRepository.Build(),
            organizacaoRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());
    }
}