using FfkApi.Application.UseCases.Equipe.Cadastrar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Equipe.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarEquipeUseCaseTest
{
    private static void AssertResponseComRequest(ResponseDadosEquipe? response, RequestCadastrarEquipe request)
    {
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Id, Is.Not.Null);
        Assert.That(response.Nome, Is.EqualTo(request.Nome));
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));
        Assert.That(response.Status, Is.EqualTo(request.Status));
        Assert.That(response.Organizacao, Is.EqualTo(request.Organizacao));

        var requestMembrosByEmail = request.Membros!
            .Where(m => m.Email != null)
            .ToDictionary(m => m.Email!);
        var responseMembrosByEmail = response.Membros.ToDictionary(m => m.Email!);

        Assert.That(responseMembrosByEmail.Keys, Is.EquivalentTo(requestMembrosByEmail.Keys!));

        foreach (var email in responseMembrosByEmail.Keys)
        {
            var responseMembro = responseMembrosByEmail[email];
            var requestMembro = requestMembrosByEmail[email];

            Assert.That(responseMembro.Id, Is.Not.Null, $"Id null para o email {email}");
            Assert.That(responseMembro.IdUsuario, Is.Not.Null, $"IdUsuario null para o email {email}");
            Assert.That(!string.IsNullOrWhiteSpace(responseMembro.Nome), $"IdUsuario null para o email {email}");
            Assert.That(responseMembro.Lider, Is.EqualTo(requestMembro.Lider), $"Lider diferente para o email {email}");
        }
    }

    [Test]
    public async Task Sucesso_Administrador_Cadastrando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarEquipeBuilder.Build();

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var useCase = CriarUseCase(cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComRequest(response, request);
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Informando_A_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuarioLogado = UsuarioBuilder.Build(organizacao: organizacaoEquipe);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);


        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;


        var useCase = CriarUseCase(cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComRequest(response, request);
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Nao_Informando_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarEquipeBuilder.Build();
        request.Organizacao = null;

        var organizacaoEquipe = OrganizacaoBuilder.Build();
        var usuarioLogado = UsuarioBuilder.Build(organizacao: organizacaoEquipe);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);


        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;


        var useCase = CriarUseCase(cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        var response = await useCase.Execute(request, cancellationToken);

        request.Organizacao = usuarioLogado.Organizacao.Nome;
        AssertResponseComRequest(response, request);
    }

    [Test]
    public async Task Erro_Nao_Administrador_Cadastrando_Para_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarEquipeBuilder.Build();

        var usuarioLogado = UsuarioBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build();
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);


        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;


        var useCase = CriarUseCase(cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Administrador_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarEquipeBuilder.Build();

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);


        request.Membros = [];


        var useCase = CriarUseCase(cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Emails_Usuarios_Nao_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarEquipeBuilder.Build();

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);


        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;


        var useCase = CriarUseCase(cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoEquipe: organizacaoEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        var emailsUsuarios = usuariosMembrosEquipe.Select(u => u.Email).ToList();
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAILS_DE_USUARIOS_NAO_ENCONTRADOS.Replace("{lista}", emailsUsuarios.ListaSepadadaPorVirgula())));
    }

    [Test]
    public async Task Erro_Nome_De_Equipe_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarEquipeBuilder.Build();

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);


        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;


        var useCase = CriarUseCase(cancellationToken: cancellationToken,
            nomeEquipe: request.Nome,
            usuarioLogado: usuarioLogado,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOME_DE_EQUIPE_JA_EXISTE_NA_ORGANIZACAO));
    }

    private static CadastrarEquipeUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        string? nomeEquipe = null,
        FfkApi.Domain.Entities.Organizacao? organizacaoEquipe = null,
        List<FfkApi.Domain.Entities.Usuario>? usuariosMembrosEquipe = null)
    {
        var equipeRepository = new EquipeRepositoryBuilder();
        if (nomeEquipe != null && organizacaoEquipe != null)
        {
            equipeRepository.SetupExisteEquipeComNomeReturnsTrue(nomeEquipe, organizacaoEquipe.Nome, cancellationToken);
        }

        var organizacaoRepository = new OrganizacaoRepositoryBuilder();
        if (organizacaoEquipe != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(organizacaoEquipe.Nome, cancellationToken);
            organizacaoRepository.SetupPegarOrganizacaoPorNomeReturnsOrganizacao(organizacaoEquipe, cancellationToken);
        }

        var usuarioRepository = new UsuarioRepositoryBuilder();
        if (usuariosMembrosEquipe != null && organizacaoEquipe != null)
        {
            usuarioRepository.SetupPegarUsuariosAptosPorEmailsReturnsUsuarios(usuariosMembrosEquipe, organizacaoEquipe.Nome, cancellationToken);
        }

        return new CadastrarEquipeUseCase(
            equipeRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            organizacaoRepository.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken),
            usuarioRepository.Build());
    }
}

