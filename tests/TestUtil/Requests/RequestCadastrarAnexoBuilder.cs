using Bogus;
using FfkApi.Communication.Requests;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestCadastrarAnexoBuilder
{
    public static RequestCadastrarAnexo Build(int? tamanhoArquivo = null)
    {
        return new Faker<RequestCadastrarAnexo>()
            .RuleFor(request => request.Nome, fake => fake.Lorem.Word())
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.Arquivo, fake => fake.FormFile(tamanho: tamanhoArquivo));
    }

    public static RequestCadastrarAnexo Build(FfkApi.Domain.Entities.Anexo anexo)
    {
        return new RequestCadastrarAnexo
        {
            Nome = anexo.Nome,
            Descricao = anexo.Descricao,
            Arquivo = new Faker().FormFile(nomeArquivo: anexo.NomeArquivo, tamanho: (int)anexo.TamanhoBytes)
        };
    }
}
