using FfkApi.Domain.Services.Mensageria;

namespace FfkApi.Infrastructure.Services.Mensageria;

public class PublicarMensagemFakeService : IPublicarMensagemService
{
    public bool EstaDisponivel()
    {
        return true;
    }

    public async Task PublicarAsync<T>(T mensagem, string nomeFila = "", CancellationToken cancellationToken = default) where T : class
    {
        await Task.Run(() => { }, cancellationToken);
    }
}
