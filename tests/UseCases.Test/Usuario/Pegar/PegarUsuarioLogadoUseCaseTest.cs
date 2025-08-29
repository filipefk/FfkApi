using FfkApi.Application.UseCases.Usuario.Pegar;
using FfkApi.Communication.Requests;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Entities;
using TestUtil.Services;

namespace UnidadeUseCases.Test.Usuario.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioLogadoUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var usuarioLogado = UsuarioBuilder.Build();

        var useCase = CriarUseCase(cancellationToken, usuarioLogado);

        var request = new RequestPegarUsuario()
        {
            Id = usuarioLogado.Id.ToString()
        };

        var response = await useCase.Execute(cancellationToken);

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
    }


    private static PegarUsuarioLogadoUseCase CriarUseCase(
        CancellationToken cancellationToken,
        FfkApi.Domain.Entities.Usuario usuarioLogado)
    {
        return new PegarUsuarioLogadoUseCase(
            UsuarioLogadoServiceBuilder.Build(usuarioLogado, cancellationToken),
            MapperBuilder.Build()
        );
    }
}
