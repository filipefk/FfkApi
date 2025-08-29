using FfkApi.Application.UseCases.Usuario.Cadastrar;
using FfkApi.Domain.Entities;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Usuario.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarUsuarioUseCaseTest
{
    [Test]
    public async Task Sucesso_Informando_Todos_Os_Campos()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, perfisAcesso: perfisAcesso, permissoes: permissoes);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Email));
        Assert.That(response.Email, Is.EqualTo(request.Email));
        Assert.That(!string.IsNullOrWhiteSpace(response.Cpf));
        Assert.That(response.Cpf, Is.EqualTo(request.Cpf));
        Assert.That(!string.IsNullOrWhiteSpace(response.Telefone));
        Assert.That(response.Telefone, Is.EqualTo(request.Telefone));
        Assert.That(!string.IsNullOrWhiteSpace(response.Status));
        Assert.That(response.Status, Is.EqualTo("Inativo"));
        Assert.That(!string.IsNullOrWhiteSpace(response.Organizacao));
        Assert.That(response.Organizacao, Is.EqualTo(organizacao.Nome));
        Assert.That(response.PerfisAcesso, Is.Not.Null);
        Assert.That(response.PerfisAcesso, Is.EquivalentTo(request.PerfisAcesso!));
        Assert.That(response.Permissoes, Is.Not.Null);
        Assert.That(response.Permissoes, Is.EquivalentTo(request.Permissoes!));
    }

    [Test]
    public async Task Sucesso_Sem_Informar_Telefone()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Telefone = null;

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, perfisAcesso: perfisAcesso, permissoes: permissoes);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Email));
        Assert.That(response.Email, Is.EqualTo(request.Email));
        Assert.That(!string.IsNullOrWhiteSpace(response.Cpf));
        Assert.That(response.Cpf, Is.EqualTo(request.Cpf));
        Assert.That(string.IsNullOrWhiteSpace(response.Telefone));
        Assert.That(!string.IsNullOrWhiteSpace(response.Status));
        Assert.That(response.Status, Is.EqualTo("Inativo"));
        Assert.That(!string.IsNullOrWhiteSpace(response.Organizacao));
        Assert.That(response.Organizacao, Is.EqualTo(organizacao.Nome));
        Assert.That(response.PerfisAcesso, Is.Not.Null);
        Assert.That(response.PerfisAcesso, Is.EquivalentTo(request.PerfisAcesso!));
        Assert.That(response.Permissoes, Is.Not.Null);
        Assert.That(response.Permissoes, Is.EquivalentTo(request.Permissoes!));
    }

    [Test]
    public async Task Sucesso_Sem_Informar_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Organizacao = null;

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, perfisAcesso: perfisAcesso, permissoes: permissoes);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Email));
        Assert.That(response.Email, Is.EqualTo(request.Email));
        Assert.That(!string.IsNullOrWhiteSpace(response.Cpf));
        Assert.That(response.Cpf, Is.EqualTo(request.Cpf));
        Assert.That(!string.IsNullOrWhiteSpace(response.Telefone));
        Assert.That(response.Telefone, Is.EqualTo(request.Telefone));
        Assert.That(!string.IsNullOrWhiteSpace(response.Status));
        Assert.That(response.Status, Is.EqualTo("Inativo"));
        Assert.That(!string.IsNullOrWhiteSpace(response.Organizacao));
        Assert.That(response.PerfisAcesso, Is.Not.Null);
        Assert.That(response.PerfisAcesso, Is.EquivalentTo(request.PerfisAcesso!));
        Assert.That(response.Permissoes, Is.Not.Null);
        Assert.That(response.Permissoes, Is.EquivalentTo(request.Permissoes!));
    }

    [Test]
    public async Task Sucesso_Sem_Perfis_Acesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        request.PerfisAcesso = null;
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, permissoes: permissoes);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Email));
        Assert.That(response.Email, Is.EqualTo(request.Email));
        Assert.That(!string.IsNullOrWhiteSpace(response.Cpf));
        Assert.That(response.Cpf, Is.EqualTo(request.Cpf));
        Assert.That(!string.IsNullOrWhiteSpace(response.Telefone));
        Assert.That(response.Telefone, Is.EqualTo(request.Telefone));
        Assert.That(!string.IsNullOrWhiteSpace(response.Status));
        Assert.That(response.Status, Is.EqualTo("Inativo"));
        Assert.That(!string.IsNullOrWhiteSpace(response.Organizacao));
        Assert.That(response.Organizacao, Is.EqualTo(organizacao.Nome));
        Assert.That(response.PerfisAcesso, Is.Not.Null);
        Assert.That(response.PerfisAcesso, Is.Empty);
        Assert.That(response.Permissoes, Is.Not.Null);
        Assert.That(response.Permissoes, Is.EquivalentTo(request.Permissoes!));
    }

    [Test]
    public async Task Sucesso_Sem_Permissoes()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        request.Permissoes = null;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, perfisAcesso: perfisAcesso);

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Nome));
        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(!string.IsNullOrWhiteSpace(response.Email));
        Assert.That(response.Email, Is.EqualTo(request.Email));
        Assert.That(!string.IsNullOrWhiteSpace(response.Cpf));
        Assert.That(response.Cpf, Is.EqualTo(request.Cpf));
        Assert.That(!string.IsNullOrWhiteSpace(response.Telefone));
        Assert.That(response.Telefone, Is.EqualTo(request.Telefone));
        Assert.That(!string.IsNullOrWhiteSpace(response.Status));
        Assert.That(response.Status, Is.EqualTo("Inativo"));
        Assert.That(!string.IsNullOrWhiteSpace(response.Organizacao));
        Assert.That(response.Organizacao, Is.EqualTo(organizacao.Nome));
        Assert.That(response.PerfisAcesso, Is.Not.Null);
        Assert.That(response.PerfisAcesso, Is.EquivalentTo(request.PerfisAcesso!));
        Assert.That(response.Permissoes, Is.Not.Null);
        Assert.That(response.Permissoes, Is.Empty);
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, perfisAcesso: perfisAcesso, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Nenhum_Perfil_Acesso_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.PERFIS_ACESSO_NAO_ENCONTRADOS.Replace("{lista}", request.PerfisAcesso!.ListaSepadadaPorVirgula())));
    }

    [Test]
    public async Task Erro_Algum_Perfil_Acesso_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        request.PerfisAcesso!.Add("PerfilAcessoInvalido");

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, perfisAcessoRequest: request.PerfisAcesso!, perfisAcesso: perfisAcesso, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.PERFIS_ACESSO_NAO_ENCONTRADOS.Replace("{lista}", "PerfilAcessoInvalido")));
    }

    [Test]
    public async Task Erro_Nenhuma_Permissao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, perfisAcesso: perfisAcesso);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.PERMISSOES_NAO_ENCONTRADAS.Replace("{lista}", request.Permissoes!.ListaSepadadaPorVirgula())));
    }

    [Test]
    public async Task Erro_Alguma_Permissao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        request.Permissoes!.Add("PermissaoInvalida");

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, perfisAcesso: perfisAcesso, permissoesRequest: request.Permissoes!, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.PERMISSOES_NAO_ENCONTRADAS.Replace("{lista}", "PermissaoInvalida")));
    }

    [Test]
    public async Task Erro_Email_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, email: request.Email, organizacao: organizacao, perfisAcesso: perfisAcesso, permissoes: permissoes);

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

        var request = RequestCadastrarUsuarioBuilder.Build();

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, cpf: request.Cpf, organizacao: organizacao, perfisAcesso: perfisAcesso, permissoes: permissoes);

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
    public async Task Erro_Email_Vazio(string? email)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarUsuarioBuilder.Build();
        request.Email = email!;

        var organizacao = OrganizacaoBuilder.Build(request.Organizacao);
        var perfisAcesso = PerfilAcessoBuilder.BuildList(request.PerfisAcesso!);
        var permissoes = PermissaoBuilder.BuildList(request.Permissoes!);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, organizacao: organizacao, perfisAcesso: perfisAcesso, permissoes: permissoes);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAIL_VAZIO));
    }

    private static CadastrarUsuarioUseCase CriarUseCase(
        CancellationToken cancellationToken,
        string? email = null,
        string? cpf = null,
        FfkApi.Domain.Entities.Organizacao? organizacao = null,
        IList<string>? perfisAcessoRequest = null,
        List<PerfilAcesso>? perfisAcesso = null,
        IList<string>? permissoesRequest = null,
        List<Permissao>? permissoes = null)
    {
        var usuarioRepository = new UsuarioRepositoryBuilder();

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

        var perfilAcessoRepository = new PerfilAcessoRepositoryBuilder();
        if (perfisAcesso != null)
        {
            perfisAcessoRequest ??= perfisAcesso.ToListNome()!;
            perfilAcessoRepository.SetupPegarPorNomesAsyncReturnsPerfis(perfisAcessoRequest, perfisAcesso, cancellationToken);
        }

        var permissaoRepository = new PermissaoRepositoryBuilder();
        if (permissoes != null)
        {
            permissoesRequest ??= permissoes.ToListNome()!;
            permissaoRepository.SetupPegarPorNomesAsyncReturnsPermissoes(permissoesRequest, permissoes, cancellationToken);
        }

        return new CadastrarUsuarioUseCase(
            usuarioRepository.Build(),
            perfilAcessoRepository.Build(),
            permissaoRepository.Build(),
            organizacaoRepository.Build(),
            UsuarioLogadoServiceBuilder.Build(UsuarioBuilder.Build(permissoes: ["Cadastro de Usuários"]), cancellationToken),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());
    }
}