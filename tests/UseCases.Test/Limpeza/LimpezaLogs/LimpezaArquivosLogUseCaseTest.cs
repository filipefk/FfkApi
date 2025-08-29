using FfkApi.Application.UseCases.Limpeza.LimpezaLogs;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace UnidadeUseCases.Test.Limpeza.LimpezaLogs;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class LimpezaArquivosLogUseCaseTest
{
    [Test]
    public async Task Sucesso_Excluiu_So_Antigos()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        uint dias = 15;

        var dataAntigo1 = DateTime.Today.AddDays(-(dias + 1));
        var dataAntigo2 = DateTime.Today.AddDays(-(dias + 2));
        var dataRecente1 = DateTime.Today.AddDays(-(dias - 1));
        var dataRecente2 = DateTime.Today.AddDays(-(dias - 2));
        var antigo1 = Path.Combine(tempDir, $"log{dataAntigo1:yyyyMMdd}.txt");
        var antigo2 = Path.Combine(tempDir, $"log{dataAntigo2:yyyyMMdd}.txt");
        var recente1 = Path.Combine(tempDir, $"log{dataRecente1:yyyyMMdd}.txt");
        var recente2 = Path.Combine(tempDir, $"log{dataRecente2:yyyyMMdd}.txt");
        File.WriteAllText(antigo1, "antigo1");
        File.WriteAllText(antigo2, "antigo2");
        File.WriteAllText(recente1, "recente1");
        File.WriteAllText(recente2, "recente2");

        var configMock = new Mock<IConfiguration>();
        var sectionMock = new Mock<IConfigurationSection>();
        sectionMock.Setup(s => s.Value).Returns(dias.ToString());
        configMock.Setup(c => c.GetSection("Configuracoes:Limpeza:LimpezaArquivos:LimpezaArquivosLogDias"))
                  .Returns(sectionMock.Object);

        var limpeza = new TestableLimpezaArquivosLog(configMock.Object, tempDir);

        var excluidos = await limpeza.Execute(CancellationToken.None);

        CollectionAssert.Contains(excluidos, antigo1);
        Assert.That(File.Exists(antigo1), Is.False);
        Assert.That(File.Exists(antigo2), Is.False);
        Assert.That(File.Exists(recente1), Is.True);
        Assert.That(File.Exists(recente2), Is.True);
        CollectionAssert.DoesNotContain(excluidos, recente1);
        CollectionAssert.DoesNotContain(excluidos, recente2);

        File.Delete(recente1);
        File.Delete(recente2);
        Directory.Delete(tempDir);
    }

    private class TestableLimpezaArquivosLog : LimpezaArquivosLogUseCase
    {
        public TestableLimpezaArquivosLog(IConfiguration config, string logDir)
            : base(config)
        {
            typeof(LimpezaArquivosLogUseCase)
                .GetField("_logDirectory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(this, logDir);
        }
    }
}