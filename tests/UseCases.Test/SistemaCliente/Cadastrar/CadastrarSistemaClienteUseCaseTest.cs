using FfkApi.Application.UseCases.SistemaCliente.Cadastrar;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using NUnit.Framework;
using TestUtil.AutoMapper;
using TestUtil.Criptografia;
using TestUtil.Repositories;
using TestUtil.Requests;

namespace UnidadeUseCases.Test.SistemaCliente.Cadastrar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CadastrarSistemaClienteUseCaseTest
{
    [Test]
    public async Task Sucesso()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarSistemaClienteBuilder.Build();

        var useCase = CriarUseCase();

        var response = await useCase.Execute(request, cancellationToken);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);
        Assert.That(response.AppId, Is.Not.Null);

        Assert.That(response.Nome, Is.Not.Null);
        Assert.That(response.Nome, Is.EqualTo(request.Nome));

        Assert.That(response.Descricao, Is.Not.Null);
        Assert.That(response.Descricao, Is.EqualTo(request.Descricao));

        Assert.That(response.Status, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo(request.Status));
    }

    [Test]
    [TestCase("Indefinido")]
    [TestCase("Excluido")]
    [TestCase("Ausente")]
    public async Task Erro_Status_Invalido(string? status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = RequestCadastrarSistemaClienteBuilder.Build();
        request.Status = status;

        var useCase = CriarUseCase();

        async Task func() => await useCase.Execute(request, cancellationToken);

        var ex = await Task.Run(() => Assert.ThrowsAsync<ErrorOnValidationException>(async () => await func()));
        Assert.That(ex, Is.Not.Null);
        var mensagensDeErro = ex!.PegarMensagensDeErro();
        Assert.That(mensagensDeErro.Count, Is.EqualTo(1));
        Assert.That(mensagensDeErro, Contains.Item(ResourceMessagesException.STATUS_INVALIDO.Replace("{ValoresPossiveis}", "Ativo, Inativo")));
    }

    private static CadastrarSistemaClienteUseCase CriarUseCase()
    {

        return new CadastrarSistemaClienteUseCase(
            new SistemaClienteRepositoryBuilder().Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build(),
            EncriptadorSenhaBuilder.Build());
    }
}
