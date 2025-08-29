using Bogus;
using FfkApi.Communication.Requests;
using TestUtil.Extension;

namespace TestUtil.Requests;

public class RequestTrocarSenhaBuilder
{
    public static RequestTrocarSenha Build(int tamanhoSenha = 10)
    {
        return new Faker<RequestTrocarSenha>()
            .RuleFor(request => request.SenhaAntiga, fake => fake.Internet.Senha())
            .RuleFor(request => request.NovaSenha, fake => fake.Internet.Senha(tamanhoSenha));
    }

    public static RequestTrocarSenha Build(string senhaAntiga, int tamanhoNovaSenha = 10)
    {
        var novaSenha = new Faker().Internet.Senha(tamanhoNovaSenha);

        return new RequestTrocarSenha
        {
            SenhaAntiga = senhaAntiga,
            NovaSenha = novaSenha
        };
    }
}
