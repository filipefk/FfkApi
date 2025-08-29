using Bogus;
using FfkApi.Domain.Entities;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class OrganizacaoBuilder
{
    public static Organizacao Build(string? nome = null)
    {
        nome ??= new Faker().NomeEmpresa();
        var remetenteEmail = "nao-responda@" + nome.Replace(" ", "").ToLower() + ".com";

        return new Faker<Organizacao>()
            .RuleFor(organizacao => organizacao.Id, () => Guid.NewGuid())
            .RuleFor(organizacao => organizacao.Nome, _ => nome)
            .RuleFor(organizacao => organizacao.Descricao, fake => fake.Proverbio())
            .RuleFor(organizacao => organizacao.RemetenteEmail, _ => remetenteEmail)
            .RuleFor(organizacao => organizacao.RemetenteNome, _ => nome)
            .RuleFor(organizacao => organizacao.ModeloEmailAtivacao, _ => "Modelo Aticação")
            .RuleFor(organizacao => organizacao.ModeloEmailNovaSenha, _ => "Modelo Nova Senha");
    }
}
