using FfkApi.Application.UseCases.Limpeza.LimpezaBanco;
using FfkApi.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using TestUtil.Repositories;

namespace UnidadeUseCases.Test.Limpeza.LimpezaBanco;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LimpezaAuditoriaSegurancaUseCaseTest
{
    [Test]
    public async Task Sucesso_Excluiu_Registros()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        uint dias = 30;
        int registrosLimpados = 5;

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, unitOfWork: unitOfWorkMock.Object, dias: dias, registrosExcluidos: registrosLimpados);

        var resultado = await useCase.Execute(cancellationToken);

        Assert.That(resultado, Is.EqualTo(registrosLimpados));
        unitOfWorkMock.Verify(u => u.CommitAsync(cancellationToken), Times.Once);
    }

    [Test]
    public async Task Sucesso_Nao_Excluiu_Registros()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        uint dias = 30;
        int registrosLimpados = 0;

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var useCase = CriarUseCase(cancellationToken: cancellationToken, unitOfWork: unitOfWorkMock.Object, dias: dias, registrosExcluidos: registrosLimpados);

        var resultado = await useCase.Execute(cancellationToken);

        Assert.That(resultado, Is.EqualTo(registrosLimpados));
        unitOfWorkMock.Verify(u => u.CommitAsync(cancellationToken), Times.Never);
    }

    private static LimpezaAuditoriaSegurancaUseCase CriarUseCase(
        CancellationToken cancellationToken,
        IUnitOfWork unitOfWork,
        uint? dias = null,
        int? registrosExcluidos = null)
    {
        var auditoriaSegurancaRepository = new AuditoriaSegurancaRepositoryBuilder();
        var configMock = new Mock<IConfiguration>();
        var sectionMock = new Mock<IConfigurationSection>();

        if (dias != null)
        {
            sectionMock.Setup(s => s.Value).Returns(dias.ToString());
            configMock.Setup(c => c.GetSection("Configuracoes:Limpeza:LimpezaBanco:LimpezaAuditoriaSegurancaDias"))
                      .Returns(sectionMock.Object);

            if (registrosExcluidos != null)
            {
                auditoriaSegurancaRepository.SetupLimparReturnsQuant(
                dias.Value,
                registrosExcluidos.Value,
                cancellationToken);
            }
        }

        return new LimpezaAuditoriaSegurancaUseCase(
            auditoriaSegurancaRepository.Build(),
            unitOfWork,
            configMock.Object);
    }
}