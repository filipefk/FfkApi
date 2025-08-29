using Bogus;
using Bogus.DataSets;
using FfkApi.Domain.Enums;
using FfkApi.Infrastructure.Security.Credenciais;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using TestUtil.Cpf;

namespace TestUtil.Extension;

public static class BogusExtensions
{
    private static readonly List<string> _proverbios = CarregarListaDeStringsJson(
        Path.Combine("Json", "Proverbio.json")
    );

    private static readonly List<string> _adjetivos = CarregarListaDeStringsJson(
        Path.Combine("Json", "Adjetivo.json")
    );

    private static readonly List<string> _animais = CarregarListaDeStringsJson(
        Path.Combine("Json", "Animal.json")
    );

    private static readonly List<string> _objetos = CarregarListaDeStringsJson(
        Path.Combine("Json", "Objeto.json")
    );

    private static readonly List<string> _verbosGerundio = CarregarListaDeStringsJson(
        Path.Combine("Json", "VerboGerundio.json")
    );

    private static readonly List<string> _indisponibilidades = CarregarListaDeStringsJson(
        Path.Combine("Json", "Indisponibilidade.json")
    );

    private static Dictionary<string, string> _tiposConteudoArquivo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [".txt"] = "text/plain",
        [".pdf"] = "application/pdf",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        [".mp3"] = "audio/mpeg",
        [".mp4"] = "video/mp4",
        [".zip"] = "application/zip"
    };

    public static List<string> CarregarListaDeStringsJson(string caminhoArquivo)
    {
        var json = File.ReadAllText(caminhoArquivo);
        return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
    }

    public static string CpfSoNumeros(this Person _)
    {
        return CpfUtil.GerarCpfSoNumerosValido();
    }

    public static string CelularBrasileiro(this Person _)
    {
        var faker = new Faker();
        return "+55" + faker.Random.String2(2, "123456789") + "9" + faker.Random.String2(1, "89") + faker.Random.String2(7, "0123456789");
    }

    public static string Senha(this Internet _, int tamanho = 8)
    {
        return new GeradorSenhaValida().GerarSenha(tamanho);
    }

    public static string AlphaNumericUrl(this Randomizer randomizer, int tamanho)
    {
        const string urlSafeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        return randomizer.String2(tamanho, urlSafeChars);
    }

    public static string NomeArquivo(this Faker faker)
    {
        var name = faker.Lorem.Sentence(faker.Random.Int(1, 3));
        var extension = faker.PickRandom(_tiposConteudoArquivo.Keys.ToList());
        return ($"{name}{extension}").Replace("..", ".");
    }

    public static string NomeEquipe(this Faker faker)
    {
        return $"Equipe {faker.Hacker.Adjective()} {faker.Hacker.IngVerb()} {faker.Hacker.Noun()} {faker.Lorem.Word()}";
    }

    public static string Proverbio(this Faker faker)
    {
        return faker.PickRandom(_proverbios);
    }

    public static string Indisponibilidade(this Faker faker)
    {
        return faker.PickRandom(_indisponibilidades);
    }

    public static string DateOnlyString(this Faker faker, DateTime? depoisDe = null)
    {
        depoisDe ??= DateTime.Now;
        int dias = faker.Random.Int(1, 30);
        return $"{depoisDe.Value.AddDays(dias):dd/MM/yyyy}";
    }

    public static string DateOnlyString(this Faker faker, string strDepoisDe)
    {
        DateTime depoisDe;
        if (!DateTime.TryParseExact(strDepoisDe, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out depoisDe))
        {
            depoisDe = DateTime.Now;
        }
        int dias = faker.Random.Int(1, 30);
        return $"{depoisDe.AddDays(dias):dd/MM/yyyy}";
    }

    public static DateOnly DateOnlyDate(this Faker faker, DateOnly? depoisDe = null)
    {
        depoisDe ??= DateOnly.FromDateTime(DateTime.Today);
        int dias = faker.Random.Int(1, 30);
        return depoisDe.Value.AddDays(dias);
    }

    public static DateOnly DateOnlyDate(this Faker faker, string strDepoisDe)
    {
        DateTime depoisDe;
        if (!DateTime.TryParseExact(strDepoisDe, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out depoisDe))
        {
            depoisDe = DateTime.Now;
        }
        int dias = faker.Random.Int(1, 30);
        return DateOnly.FromDateTime(depoisDe.AddDays(dias));
    }

    public static string Adjetivo(this Faker faker)
    {
        return faker.PickRandom(_adjetivos);
    }

    public static string Animal(this Faker faker)
    {
        return faker.PickRandom(_animais);
    }

    public static string Objeto(this Faker faker)
    {
        return faker.PickRandom(_objetos);
    }

    public static string VerboGerundio(this Faker faker)
    {
        return faker.PickRandom(_verbosGerundio);
    }

    public static string Palavras(this Lorem loren, int quant = 3)
    {
        return string.Join(" ", loren.Words(quant));
    }

    public static T PickRandomEnum<T>(this Faker faker, List<T>? removerEstes = null) where T : Enum
    {
        return faker.PickRandom(EnumUtil.PegarListaEnum<T>(removerEstes));
    }

    public static IFormFile FormFile(this Faker faker, string? nomeArquivo = null, string? tipoConteudo = null, int? tamanho = null)
    {
        nomeArquivo ??= faker.NomeArquivo();
        tamanho ??= faker.Random.Int(100, 300);
        var conteudoArquivo = LorenTexto(tamanho.Value);

        var extensao = Path.GetExtension(nomeArquivo);
        tipoConteudo ??= TipoConteudoArquivo(extensao);

        var bytes = Encoding.UTF8.GetBytes(conteudoArquivo);
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "Arquivo", nomeArquivo)
        {
            Headers = new HeaderDictionary(),
            ContentType = tipoConteudo
        };
    }

    private static string LorenTexto(int? tamanho)
    {
        var faker = new Faker();
        tamanho ??= faker.Random.Int(100, 300);

        var sb = new StringBuilder();

        sb.Append(faker.Lorem.Sentence());

        while (sb.Length < tamanho.Value)
        {
            sb.Append(faker.Lorem.Sentence());
        }

        if (sb.Length > tamanho)
        {
            sb.Length = tamanho.Value;
        }

        return sb.ToString();
    }

    public static string TipoConteudoArquivo(this Faker _, string extensao)
    {
        return TipoConteudoArquivo(extensao);
    }

    private static string TipoConteudoArquivo(string extensao)
    {
        return _tiposConteudoArquivo.TryGetValue(extensao, out var ct) ? ct : "application/octet-stream";
    }

    public static string NomeEmpresa(this Faker faker)
    {
        return $"{faker.Company.CompanyName()} {faker.Lorem.Palavras(3)}";
    }
}