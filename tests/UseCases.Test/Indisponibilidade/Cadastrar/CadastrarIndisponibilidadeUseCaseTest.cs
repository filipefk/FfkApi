using FfkApi.Application.UseCases.Indisponibilidade.Cadastrar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Requests;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Indisponibilidade.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarIndisponibilidadeUseCaseTest
{
    private void AssertResponseComRequest(ResponseDadosIndisponibilidade response, RequestCadastrarIndisponibilidade request)
    {
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));
        Assert.That(response.DataInicial, Is.EqualTo(request.DataInicial));
        Assert.That(response.DataFinal, Is.EqualTo(request.DataFinal));
        Assert.That(response.Usuario.Id, Is.Not.Null);
        Assert.That(!string.IsNullOrWhiteSpace(response.Usuario.Nome));
        Assert.That(response.Usuario.Email, Is.EqualTo(request.Usuario));
        Assert.That(!string.IsNullOrWhiteSpace(response.Usuario.Organizacao));
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build();

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioIndisponibilidade: usuarioIndisponibilidade);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComRequest(response, request);
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(permissoes: ["Cadastro de Indisponibilidades"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build(organizacao: usuarioLogado.Organizacao);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioIndisponibilidade: usuarioIndisponibilidade);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComRequest(response, request);
    }

    [Test]
    public async Task Sucesso_Usuario_Sem_Permissao_Cadastrando_Pra_Si_Mesmo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioLogado.Email;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioIndisponibilidade: usuarioLogado);

        var response = await useCase.Execute(request, cancellationToken);

        AssertResponseComRequest(response, request);
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Cadastrando_Para_Outro_Usuario()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();
        var usuarioIndisponibilidade = UsuarioBuilder.Build(organizacao: usuarioLogado.Organizacao);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioIndisponibilidade: usuarioIndisponibilidade);

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

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.USUARIO_NAO_ENCONTRADO));
    }

    [Test]
    public async Task Erro_Existe_Indisponibilidade_Para_Usuario_No_Periodo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build(organizacao: usuarioLogado.Organizacao);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;

        var useCase = CriarUseCase(
            cancellationToken: cancellationToken,
            usuarioLogado: usuarioLogado,
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
    public async Task Erro_Descricao_Vazia(string? descricao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build(perfisAcesso: ["Administrador"]);
        var usuarioIndisponibilidade = UsuarioBuilder.Build(organizacao: usuarioLogado.Organizacao);

        var request = RequestCadastrarIndisponibilidadeBuilder.Build();
        request.Usuario = usuarioIndisponibilidade.Email;
        request.Descricao = descricao;

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarioLogado: usuarioLogado, usuarioIndisponibilidade: usuarioIndisponibilidade);

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.DESCRICAO_VAZIA));
    }

    private static CadastrarIndisponibilidadeUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado,
        bool existeIndisponibilidadeParaUsuarioNoPeriodo = false,
        FfkApi.Domain.Entities.Usuario? usuarioIndisponibilidade = null)
    {
        var indisponibilidadeRepository = new IndisponibilidadeRepositoryBuilder();
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

        return new CadastrarIndisponibilidadeUseCase(
            indisponibilidadeRepository.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken),
            usuarioRepository.Build());
    }
}
