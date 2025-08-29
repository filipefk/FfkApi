namespace FfkApi.Domain.Services.Mensageria;

public interface IPublicarMensagemService
{
    Task PublicarAsync<T>(T mensagem, string nomeFila = "", CancellationToken cancellationToken = default)
        where T : class;

    bool EstaDisponivel();
}
