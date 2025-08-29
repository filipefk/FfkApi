using System.Net;
using System.Text.Json;
using TestUtil.HttpUtil;

namespace Aceitacao.Test.HealthCheck;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EHealthCheckTest : E2EClassFixture
{
    private const string _appToken = "healthcheck-KgbWQqFa9UrMzJvBmfyds4hq4-UkoekbWi56wsYYocw";
    private readonly string[] _detailComponentList = ["PostgreSQL", "Mensageria", "Email", "Armazenador de arquivos"];
    private readonly string[] _readyComponentList = ["PostgreSQL", "Armazenador de arquivos"];
    private readonly string _baseUrlHealthCheck = "health";

    [Test]
    public async Task Sucesso_Healt()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet(url: _baseUrlHealthCheck, cancellationToken: cancellationToken, appToken: _appToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var content = await response.Content.ReadAsStringAsync();

        Assert.That(content, Does.Contain("Healthy"));
    }

    [Test]
    public async Task Sucesso_Healt_Live()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet(url: $"{_baseUrlHealthCheck}/live", cancellationToken: cancellationToken, appToken: _appToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var content = await response.Content.ReadAsStringAsync();

        Assert.That(content, Does.Contain("Healthy"));
    }

    [Test]
    public async Task Sucesso_Healt_Detail()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet(url: $"{_baseUrlHealthCheck}/detail", cancellationToken: cancellationToken, appToken: _appToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo("Healthy"));

        var healthChecks = dadosDaResposta.RootElement.GetProperty("checks").EnumerateArray();
        Assert.That(healthChecks.Count(), Is.EqualTo(_detailComponentList.Length));

        foreach (var componente in _detailComponentList)
        {
            var healthCheck = healthChecks.FirstOrDefault(h => h.GetProperty("component").GetString() == componente);
            Assert.That(healthCheck.ValueKind, Is.EqualTo(JsonValueKind.Object));
            var statusComponente = healthCheck.GetProperty("status").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(statusComponente));
            Assert.That(statusComponente, Is.EqualTo("Healthy"));
        }
    }

    [Test]
    public async Task Sucesso_Healt_Ready()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet(url: $"{_baseUrlHealthCheck}/ready", cancellationToken: cancellationToken, appToken: _appToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo("Healthy"));

        var healthChecks = dadosDaResposta.RootElement.GetProperty("checks").EnumerateArray();
        Assert.That(healthChecks.Count(), Is.EqualTo(_readyComponentList.Length));

        foreach (var componente in _readyComponentList)
        {
            var healthCheck = healthChecks.FirstOrDefault(h => h.GetProperty("component").GetString() == componente);
            Assert.That(healthCheck.ValueKind, Is.EqualTo(JsonValueKind.Object));
            var statusComponente = healthCheck.GetProperty("status").GetString();
            Assert.That(!string.IsNullOrWhiteSpace(statusComponente));
            Assert.That(statusComponente, Is.EqualTo("Healthy"));
        }
    }
}
