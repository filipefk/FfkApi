using FfkApi.Domain.Services.Mensageria;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FfkApi.Infrastructure.Services.Mensageria;

public class RabbitMqPublicarService : IPublicarMensagemService
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublicarService> _logger;

    public RabbitMqPublicarService(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublicarService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublicarAsync<T>(T mensagem, string _ = "", CancellationToken cancellationToken = default) where T : class
    {
        if (!EstaDisponivel())
        {
            _logger.LogWarning("RabbitMQ não está disponível. Não foi possível publicar a mensagem.");
            return Task.CompletedTask;
        }

        var json = JsonSerializer.Serialize(mensagem);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _channel!.CreateBasicProperties();
        props.Persistent = true;

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _options.NomeFila,
            basicProperties: props,
            body: body
        );

        _logger.LogInformation("Mensagem publicada: nomeFila={NomeFila} conteúdo={Json}", _options.NomeFila, json);

        return Task.CompletedTask;
    }

    private void TentarConectar()
    {
        if (_connection?.IsOpen == true && _channel?.IsOpen == true)
            return;

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Porta,
            UserName = _options.Usuario,
            Password = _options.Senha,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _options.NomeFila,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    public bool EstaDisponivel()
    {
        try
        {
            TentarConectar();
            return _connection?.IsOpen == true && _channel?.IsOpen == true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "RabbitMQ indisponível");
            return false;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
