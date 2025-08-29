using FfkApi.Application.UseCases.Equipe.Alterar;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Equipe.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarEquipeUseCaseTest
{
    [Test]
    public async Task Sucesso_Administrador_Alterando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Administrador_Alterando_A_Organizacao_De_Uma_Equipe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoRequest = OrganizacaoBuilder.Build(nome: request.Organizacao);
        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: "Organizacao da Equipe");
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);

        request.Membros = [];

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            organizacaoEquipe: organizacaoRequest);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Alterando_Uma_Equipe_Da_Mesma_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build(organizacao: organizacaoEquipe);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Sucesso_Nao_Administrador_Nao_Informando_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();
        request.Organizacao = null;

        var organizacaoEquipe = OrganizacaoBuilder.Build();
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build(organizacao: organizacaoEquipe);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        await Task.Run(() => Assert.DoesNotThrowAsync(func));
    }

    [Test]
    public async Task Erro_Nao_Administrador_Alterando_Equipe_De_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build();

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(2));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EQUIPE_NAO_ENCONTRADA));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Administrador_Alterando_A_Organizacao_De_Uma_Equipe_Que_Tem_Membros()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build();
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);
        var novaOrganizacao = OrganizacaoBuilder.Build(request.Organizacao);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            organizacaoEquipe: organizacaoEquipe,
            novaOrganizacao: novaOrganizacao,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(2));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.IMPOSSIVEL_TROCAR_ORGANIZACAO_EQUIPE_QUANDO_TEM_MEMBROS));

        var emails = request.Membros.Select(m => m.Email).ToList();
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAILS_DE_USUARIOS_NAO_ENCONTRADOS.Replace("{lista}", emails!.ListaSepadadaPorVirgula())));
    }

    [Test]
    public async Task Erro_Equipe_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EQUIPE_NAO_ENCONTRADA));
    }

    [Test]
    public async Task Erro_Nome_Equipe_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            nomeEquipeMesmoNome: request.Nome,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.NOME_DE_EQUIPE_JA_EXISTE_NA_ORGANIZACAO));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = [];

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

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

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
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
    public async Task Erro_Email_Membro_Equipe_Repetido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestAlterarEquipeBuilder.Build();

        var organizacaoEquipe = OrganizacaoBuilder.Build(nome: request.Organizacao!);
        var equipeAlteracao = EquipeBuilder.Build(organizacao: organizacaoEquipe);
        equipeAlteracao.Id = Guid.Parse(request.Id!);
        var usuariosMembrosEquipe = UsuarioBuilder.BuildList(organizacao: organizacaoEquipe);
        usuariosMembrosEquipe.Add(usuariosMembrosEquipe[0]);

        request.Membros = RequestMembroEquipeBuilder.BuildList(usuariosMembrosEquipe);
        request.Membros[0].Lider = true;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
            equipeAlteracao: equipeAlteracao,
            organizacaoEquipe: organizacaoEquipe,
            usuariosMembrosEquipe: usuariosMembrosEquipe);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.EMAIL_MEMBRO_EQUIPE_REPETIDOS));
    }

    private static AlterarEquipeUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        FfkApi.Domain.Entities.Equipe? equipeAlteracao = null,
        string? nomeEquipeMesmoNome = null,
        FfkApi.Domain.Entities.Organizacao? organizacaoEquipe = null,
        FfkApi.Domain.Entities.Organizacao? novaOrganizacao = null,
        List<FfkApi.Domain.Entities.Usuario>? usuariosMembrosEquipe = null)
    {
        var equipeRepository = new EquipeRepositoryBuilder();

        if (equipeAlteracao != null)
        {
            equipeRepository.SetupPegarEquipePorIdReturnsEquipe(equipeAlteracao, cancellationToken);
            if (organizacaoEquipe != null)
                equipeRepository.SetupPegarEquipePorIdReturnsEquipe(equipeAlteracao, organizacaoEquipe.Id, cancellationToken);
        }

        if (nomeEquipeMesmoNome != null && organizacaoEquipe != null)
        {
            equipeRepository.SetupExisteEquipeComNomeReturnsTrue(nomeEquipeMesmoNome, organizacaoEquipe.Nome, cancellationToken);
        }

        var organizacaoRepository = new OrganizacaoRepositoryBuilder();
        if (organizacaoEquipe != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(organizacaoEquipe.Nome, cancellationToken);
            organizacaoRepository.SetupPegarOrganizacaoPorNomeReturnsOrganizacao(organizacaoEquipe, cancellationToken);
        }
        if (novaOrganizacao != null)
        {
            organizacaoRepository.SetupExisteOrganizacaoComNomeReturnsTrue(novaOrganizacao.Nome, cancellationToken);
            organizacaoRepository.SetupPegarOrganizacaoPorNomeReturnsOrganizacao(novaOrganizacao, cancellationToken);
        }

        var usuarioRepository = new UsuarioRepositoryBuilder();
        if (usuariosMembrosEquipe != null && organizacaoEquipe != null)
        {
            usuarioRepository.SetupPegarUsuariosAptosPorEmailsReturnsUsuarios(usuariosMembrosEquipe, organizacaoEquipe.Nome, cancellationToken);
            foreach (var usuario in usuariosMembrosEquipe)
            {
                usuarioRepository.SetupPegarUsuarioAptoPorEmailReturnsUsuario(usuario, cancellationToken);
            }
        }

        return new AlterarEquipeUseCase(
            equipeRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            organizacaoRepository.Build(),
            usuarioRepository.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken));
    }
}