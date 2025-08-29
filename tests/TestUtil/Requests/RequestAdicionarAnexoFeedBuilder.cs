using Bogus;
using FfkApi.Communication.Requests;

namespace TestUtil.Requests;

public class RequestAdicionarAnexoFeedBuilder
{
    public static RequestAdicionarAnexoFeed Build(string? idFeed = null, int? tamanhoArquivo = null)
    {
        var requestCadastrarAnexo = RequestCadastrarAnexoBuilder.Build(tamanhoArquivo);

        return new Faker<RequestAdicionarAnexoFeed>()
            .RuleFor(r => r.Id, idFeed ?? Guid.NewGuid().ToString())
            .RuleFor(r => r.Nome, requestCadastrarAnexo.Nome)
            .RuleFor(r => r.Descricao, requestCadastrarAnexo.Descricao)
            .RuleFor(r => r.Arquivo, requestCadastrarAnexo.Arquivo);
    }

    public static RequestAdicionarAnexoFeed Build(FfkApi.Domain.Entities.Anexo anexo, string? idFeed = null)
    {
        var requestCadastrarAnexo = RequestCadastrarAnexoBuilder.Build(anexo);

        return new RequestAdicionarAnexoFeed
        {
            Id = idFeed ?? Guid.NewGuid().ToString(),
            Nome = requestCadastrarAnexo.Nome,
            Descricao = requestCadastrarAnexo.Descricao,
            Arquivo = requestCadastrarAnexo.Arquivo
        };
    }
}