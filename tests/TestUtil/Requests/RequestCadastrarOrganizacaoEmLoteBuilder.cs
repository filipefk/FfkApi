using FfkApi.Communication.Requests;

namespace TestUtil.Requests;

public class RequestCadastrarOrganizacaoEmLoteBuilder
{
    public static RequestCadastrarOrganizacaoEmLote Build(int? quantos = 3)
    {
        var organizacoes = new List<RequestCadastrarOrganizacao>();

        for (var i = 0; i < quantos; i++)
        {
            organizacoes.Add(RequestCadastrarOrganizacaoBuilder.Build());
        }

        return new RequestCadastrarOrganizacaoEmLote
        {
            Organizacoes = organizacoes
        };
    }

    public static RequestCadastrarOrganizacaoEmLote Build(FfkApi.Domain.Entities.Organizacao[] organizacoes)
    {
        return new RequestCadastrarOrganizacaoEmLote
        {
            Organizacoes = organizacoes.Select(RequestCadastrarOrganizacaoBuilder.Build).ToList()
        };
    }
}
