using FfkApi.Communication.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.Documentation.Examples;

public class Response200OKPesquisarEquipeExample : IExamplesProvider<ResponsePaginado<ResponseDadosEquipe>>
{
    public ResponsePaginado<ResponseDadosEquipe> GetExamples()
    {
        var resultados = new List<ResponseDadosEquipe>
        {
            new ResponseDadosEquipe
            {
                Id = Guid.NewGuid(),
                Nome = "Equipe Atlas",
                Descricao = "A equipe que carrega o mundo nas costas",
                Status = "Ativa",
                Organizacao = "Banco de Desenvolvimento Econômico",
                Membros = new List<ResponseMembroEquipe>
                {
                    new ResponseMembroEquipe
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = Guid.NewGuid(),
                        Nome = "Alexys",
                        Email = "Alexys.Fahey15@gmail.com",
                        Lider = false
                    },
                    new ResponseMembroEquipe
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = Guid.NewGuid(),
                        Nome = "Ally",
                        Email = "Ally_Upton87@hotmail.com",
                        Lider = true
                    }
                }
            },
            new ResponseDadosEquipe
            {
                Id = Guid.NewGuid(),
                Nome = "Equipe Lobo",
                Descricao = "A equipe que trabalha como uma matilha",
                Status = "Ativa",
                Organizacao = "Banco de Desenvolvimento Econômico",
                Membros = new List<ResponseMembroEquipe>
                {
                    new ResponseMembroEquipe
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = Guid.NewGuid(),
                        Nome = "Alysha",
                        Email = "Alysha.Swaniawski1@yahoo.com",
                        Lider = true
                    },
                    new ResponseMembroEquipe
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = Guid.NewGuid(),
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