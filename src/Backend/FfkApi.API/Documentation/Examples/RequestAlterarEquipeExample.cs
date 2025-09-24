using FfkApi.Communication.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.Documentation.Examples;

public class RequestAlterarEquipeExample : IExamplesProvider<RequestAlterarEquipe>
{
    public RequestAlterarEquipe GetExamples()
    {
        return new RequestAlterarEquipe
        {
            Id = Guid.NewGuid().ToString(),
            Nome = "Equipe Pescadores",
            Descricao = "O mar sempre tem razão",
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