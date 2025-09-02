using FfkApi.Communication.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.ExamplesProvider.Examples;

public class Response200OKPesquisarEquipeExample : IExamplesProvider<ResponsePaginado<ResponseDadosEquipe>>
{
    public ResponsePaginado<ResponseDadosEquipe> GetExamples()
    {
        var resultados = new List<ResponseDadosEquipe>
        {
            new ResponseDadosEquipe
            {
                Id = Guid.Parse("4461f30a-d642-4dd6-8c1f-15f715af87c7"),
                Nome = "Equipe Atlas",
                Descricao = "A equipe que carrega o mundo nas costas",
                Status = "Ativa",
                Organizacao = "Banco de Desenvolvimento Econômico",
                Membros = new List<ResponseMembroEquipe>
                {
                    new ResponseMembroEquipe
                    {
                        Nome = "Alexys",
                        Email = "Alexys.Fahey15@gmail.com",
                        Lider = false
                    },
                    new ResponseMembroEquipe
                    {
                        Nome = "Ally",
                        Email = "Ally_Upton87@hotmail.com",
                        Lider = true
                    }
                }
            },
            new ResponseDadosEquipe
            {
                Id = Guid.Parse("cea07de3-8f36-4eca-b2f0-3e3b97f1bbad"),
                Nome = "Equipe Lobo",
                Descricao = "A equipe que trabalha como uma matilha",
                Status = "Ativa",
                Organizacao = "Banco de Desenvolvimento Econômico",
                Membros = new List<ResponseMembroEquipe>
                {
                    new ResponseMembroEquipe
                    {
                        Nome = "Alysha",
                        Email = "Alysha.Swaniawski1@yahoo.com",
                        Lider = true
                    },
                    new ResponseMembroEquipe
                    {
                        Nome = "Ernesto",
                        Email = "Ernesto20@yahoo.com",
                        Lider = false
                    }
                }
            }
        };

        return new ResponsePaginado<ResponseDadosEquipe>(
            resultados,
            paginaAtual: 1,
            tamanhoDaPagina: 2,
            quantidadeTotal: 2
        );
    }
}