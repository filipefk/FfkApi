using FfkApi.Application.UseCases.EnvioEmail.EnvioEmailAtivacao;
using FfkApi.Domain.Services.Email;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Repositories;
using TestUtil.Services;
using TestUtil.Tokens;

namespace UnidadeUseCases.Test.EnvioEmailAtivacao;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class EnvioEmailAtivacaoUseCaseTest
{
    [Test]
    public async Task Sucesso_Nenhum_Envio()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var useCase = CriarUseCase(cancellationToken: cancellationToken);

        var listaResponseUsuarios = await useCase.Execute(cancellationToken);

        Assert.That(listaResponseUsuarios, Is.Null);
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async Task Sucesso_Envio(int quantosEnvios)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarios = new List<FfkApi.Domain.Entities.Usuario>();

        for (int i = 0; i < quantosEnvios; i++)
        {
            var usuario = UsuarioBuilder.Build();
            usuario.Status = FfkApi.Domain.Enums.StatusUsuario.Inativo;
            usuarios.Add(usuario);
        }

        var useCase = CriarUseCase(cancellationToken: cancellationToken, usuarios: usuarios);

        var listaResponseUsuarios = await useCase.Execute(cancellationToken);

        Assert.That(listaResponseUsuarios, Is.Not.Null);
        Assert.That(listaResponseUsuarios, Has.Count.EqualTo(quantosEnvios));
    }

    private static EnvioEmailAtivacaoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        List<FfkApi.Domain.Entities.Usuario>? usuarios = null)
    {
        var usuarioRepository = new UsuarioRepositoryBuilder();
        var tokenAtivacaoRepository = new TokenAtivacaoRepositoryBuilder();

        if (usuarios != null)
        {
            usuarioRepository.SetupPegarUsuariosParaAtivacaoReturnsUsuarios(usuarios, cancellationToken);
            usuarioRepository.SetupPegarUsuariosParaEnvioEmailAtivacaoReturnsUsuarios(usuarios, cancellationToken);
            var tokenAtivacao = TokenAtivacaoBuilder.Build();
            tokenAtivacaoRepository.SetupPegarTokenAtivacaoPorUsuarioReturnsTokenAtivacao(tokenAtivacao, cancellationToken);
        }

        var enviarEmailService = new EnviarEmailServiceBuilder();
        var resposta = new RespostaEnvioEmail
        {
            Enviado = true
        };
        enviarEmailService.SetupEnviarEmailAsyncReturnsRespostaEnvioEmail(resposta);

        return new EnvioEmailAtivacaoUseCase(
            usuarioRepository.Build(),
            tokenAtivacaoRepository.Build(),
            new GeradorTokenAtivacaoBuilder().Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            enviarEmailService.Build());
    }
}
