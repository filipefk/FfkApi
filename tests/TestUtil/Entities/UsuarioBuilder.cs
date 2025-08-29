using Bogus;
using FfkApi.Domain.Entities;
using FfkApi.Domain.Enums;
using TestUtil.Extension;

namespace TestUtil.Entities;

public class UsuarioBuilder
{
    public static Usuario Build(List<string>? perfisAcesso = null, List<string>? permissoes = null, Organizacao? organizacao = null!)
    {
        organizacao ??= OrganizacaoBuilder.Build();

        return new Faker<Usuario>()
            .RuleFor(usuario => usuario.Id, () => Guid.NewGuid())
            .RuleFor(usuario => usuario.Nome, fake => fake.Name.FirstName())
            .RuleFor(usuario => usuario.Email, (fake, usuario) => fake.Internet.Email(usuario.Nome))
            .RuleFor(usuario => usuario.Senha, fake => fake.Internet.Senha())
            .RuleFor(usuario => usuario.Cpf, fake => fake.Person.CpfSoNumeros())
            .RuleFor(usuario => usuario.Status, _ => StatusUsuario.Ativo)
            .RuleFor(usuario => usuario.Telefone, fake => fake.Person.CelularBrasileiro())
            .RuleFor(usuario => usuario.PerfisAcesso, _ =>
                perfisAcesso?.Select(perfil => new PerfilAcesso
                {
                    Nome = perfil,
                }).ToList() ?? [])
            .RuleFor(usuario => usuario.Permissoes, _ =>
                permissoes?.Select(permissao => new Permissao
                {
                    Nome = permissao,
                }).ToList() ?? [])
            .RuleFor(usuario => usuario.IdOrganizacao, _ => organizacao.Id)
            .RuleFor(usuario => usuario.Organizacao, _ => organizacao);
    }

    public static List<Usuario> BuildList(int quant = 3, List<string>? perfisAcesso = null, List<string>? permissoes = null, Organizacao? organizacao = null!)
    {
        var lista = new List<Usuario>();
        organizacao ??= OrganizacaoBuilder.Build();
        for (int i = 0; i < quant; i++)
        {
            lista.Add(Build(perfisAcesso, permissoes, organizacao));
        }
        return lista;
    }
}
