using Bogus;
using FfkApi.Communication.Requests;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestCadastrarOrganizacaoBuilder
{
    public static RequestCadastrarOrganizacao Build(string? nome = null)
    {
        nome ??= new Faker().NomeEmpresa();
        var remetenteEmail = "nao-responda@" + nome.Replace(" ", "").ToLower() + ".com";

        return new Faker<RequestCadastrarOrganizacao>()
            .RuleFor(request => request.Nome, _ => nome)
            .RuleFor(request => request.Descricao, fake => fake.Proverbio())
            .RuleFor(request => request.RemetenteEmail, _ => remetenteEmail)
            .RuleFor(request => request.RemetenteNome, _ => nome)
            .RuleFor(request => request.ModeloEmailAtivacao, _ => "Modelo Aticação")
            .RuleFor(request => request.ModeloEmailNovaSenha, _ => "Modelo Nova Senha");
    }

    public static RequestCadastrarOrganizacao Build(FfkApi.Domain.Entities.Organizacao organizacao)
    {
        return new RequestCadastrarOrganizacao
        {
            Nome = organizacao.Nome,
            Descricao = organizacao.Descricao,
            RemetenteEmail = organizacao.RemetenteEmail,
            RemetenteNome = organizacao.RemetenteNome,
            ModeloEmailAtivacao = organizacao.ModeloEmailAtivacao,
            ModeloEmailNovaSenha = organizacao.ModeloEmailNovaSenha
        };
    }
}
