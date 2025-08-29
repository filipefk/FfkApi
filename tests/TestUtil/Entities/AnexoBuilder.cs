using Bogus;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class AnexoBuilder
{
    public static Anexo Build()
    {
        var faker = new Faker();
        var nomeArquivo = faker.NomeArquivo();
        var extensao = Path.GetExtension(nomeArquivo);
        var nomeArquivoArmazenamento = $"{Guid.NewGuid()}{extensao}";

        return new Faker<Anexo>()
            .RuleFor(anexo => anexo.Id, () => Guid.NewGuid())
            .RuleFor(request => request.Nome, fake => fake.Lorem.Word())
            .RuleFor(organizacao => organizacao.Descricao, fake => fake.Proverbio())
            .RuleFor(organizacao => organizacao.NomeArquivo, _ => nomeArquivo)
            .RuleFor(organizacao => organizacao.NomeArquivoArmazenamento, _ => nomeArquivoArmazenamento)
            .RuleFor(organizacao => organizacao.Extensao, _ => extensao)
            .RuleFor(organizacao => organizacao.TamanhoBytes, fake => fake.Random.Long(100, 300))
            .RuleFor(organizacao => organizacao.MimeType, fake => fake.TipoConteudoArquivo(extensao));
    }
}