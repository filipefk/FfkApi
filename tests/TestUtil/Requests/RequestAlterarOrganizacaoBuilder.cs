using Bogus;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestAlterarOrganizacaoBuilder
{
    public static RequestAlterarOrganizacao Build(string? nome = null)
    {
        nome ??= new Faker().NomeEmpresa();
        var remetenteEmail = "nao-responda@" + nome.Replace(" ", "").ToLower() + ".com";

        return new Faker<RequestAlterarOrganizacao>()
            .RuleFor(request => request.Id, fake => fake.Random.Guid().ToString())
            .RuleFor(request => request.Nome, _ => nome)
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.RemetenteEmail, _ => remetenteEmail)
            .RuleFor(request => request.RemetenteNome, _ => nome)
            .RuleFor(request => request.ModeloEmailAtivacao, _ => "Modelo Aticação")
            .RuleFor(request => request.ModeloEmailNovaSenha, _ => "Modelo Nova Senha");
    }

    public static RequestAlterarOrganizacao Build(FfkApi.Domain.Entities.Organizacao organizacao)
    {
        return new RequestAlterarOrganizacao
        {
            Id = organizacao.Id.ToString(),
            Nome = organizacao.Nome,
            Descricao = organizacao.Descricao,
            RemetenteEmail = organizacao.RemetenteEmail,
            RemetenteNome = organizacao.RemetenteNome,
            ModeloEmailAtivacao = organizacao.ModeloEmailAtivacao,
            ModeloEmailNovaSenha = organizacao.ModeloEmailNovaSenha
        };
    }
}
