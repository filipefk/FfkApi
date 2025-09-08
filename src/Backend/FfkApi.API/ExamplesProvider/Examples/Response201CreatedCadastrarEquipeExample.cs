using FfkApi.Communication.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.ExamplesProvider.Examples;

public class Response201CreatedCadastrarEquipeExample : IExamplesProvider<ResponseDadosEquipe>
{
    public ResponseDadosEquipe GetExamples()
    {
        return new ResponseDadosEquipe
        {
            Id = Guid.Parse("0c9d79b1-e797-4422-8bab-cec46a70505f"),
            Nome = "Equipe Pescadores",
            Descricao = "Caiu na rede é peixe",
            Status = "Ativa",
            Organizacao = "Banco de Desenvolvimento Econômico",
            Membros = new List<ResponseMembroEquipe>
            {
                new ResponseMembroEquipe
                {
                    Id = Guid.Parse("8bc3a07d-69e0-4c23-b940-36156ebbe7bd"),
                    IdUsuario = Guid.Parse("ae8fc60b-d002-4ad1-a94a-abbe0ab74fb5"),
                    Nome = "SemPerfilNemPermissao",
                    Email = "SemPerfilNemPermissao@provedor.com",
                    Lider = false
                },
                new ResponseMembroEquipe
                {
                    Id = Guid.Parse("cfcd38c4-6a04-4e35-9557-feea0c32414f"),
                    IdUsuario = Guid.Parse("f5d58011-b223-4107-98d8-4abdf367468a"),
                    Nome = "PermissaoCadastroUsuarios",
                    Email = "PermissaoCadastroUsuarios@provedor.com",
                    Lider = true
                }
            }
        };
    }
}