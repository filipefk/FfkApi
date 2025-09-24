using FfkApi.Communication.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.Documentation.Examples;

public class RequestCadastrarEquipeExample : IExamplesProvider<RequestCadastrarEquipe>
{
    public RequestCadastrarEquipe GetExamples()
    {
        return new RequestCadastrarEquipe
        {
            Nome = "Equipe Pescadores",
            Descricao = "Caiu na rede é peixe",
            Status = "Ativa",
            Membros = new List<RequestMembroEquipe>
            {
                new RequestMembroEquipe
                {
                    Email = "SemPerfilNemPermissao@provedor.com",
                    Lider = false
                },
                new RequestMembroEquipe
                {
                    Email = "PermissaoCadastroUsuarios@provedor.com",
                    Lider = true
                }
            },
            Organizacao = "Banco de Desenvolvimento Econômico"
        };
    }
}