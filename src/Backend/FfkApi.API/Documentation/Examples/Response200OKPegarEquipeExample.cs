using FfkApi.Communication.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.Documentation.Examples;

public class Response200OKPegarEquipeExample : IExamplesProvider<ResponseDadosEquipe>
{
    public ResponseDadosEquipe GetExamples()
    {
        return new ResponseDadosEquipe
        {
            Id = Guid.NewGuid(),
            Nome = "Equipe Pescadores",
            Descricao = "Caiu na rede é peixe",
            Status = "Ativa",
            Organizacao = "Banco de Desenvolvimento Econômico",
            Membros = new List<ResponseMembroEquipe>
            {
                new ResponseMembroEquipe
                {
                    Id = Guid.NewGuid(),
                    IdUsuario = Guid.NewGuid(),
                    Nome = "SemPerfilNemPermissao",
                    Email = "SemPerfilNemPermissao@provedor.com",
                    Lider = false
                },
                new ResponseMembroEquipe
                {
                    Id = Guid.NewGuid(),
                    IdUsuario = Guid.NewGuid(),
                    Nome = "PermissaoCadastroUsuarios",
                    Email = "PermissaoCadastroUsuarios@provedor.com",
                    Lider = true
                }
            }
        };
    }
}