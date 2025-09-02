using FfkApi.Communication.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace FfkApi.API.ExamplesProvider.Examples;

public class RequestAlterarEquipeExample : IExamplesProvider<RequestAlterarEquipe>
{
    public RequestAlterarEquipe GetExamples()
    {
        return new RequestAlterarEquipe
        {
            Id = "3d81ab48-2337-488b-835f-27ad083967cb",
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