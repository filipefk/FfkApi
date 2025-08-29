using FfkApi.Domain.Enums;

namespace FfkApi.Domain.Entities;

public class Usuario : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Senha { get; set; } = null;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; } = null;
    public StatusUsuario Status { get; set; } = StatusUsuario.Indefinido;
    public Fila? Fila { get; set; } = null;
    public ICollection<PerfilAcesso> PerfisAcesso { get; set; } = [];
    public ICollection<Permissao> Permissoes { get; set; } = [];
    public Guid IdOrganizacao { get; set; } = default!;
    public Organizacao Organizacao { get; set; } = default!;

    public bool TemPerfilAdministrador()
    {
        return TemPerfil("Administrador");
    }

    public bool TemPerfil(string nomePerfil)
    {
        return this.PerfisAcesso.Any(p => p.Nome.Equals(nomePerfil, StringComparison.OrdinalIgnoreCase));
    }

    public bool TemPermissao(string nomePermissao)
    {
        if (TemPerfilAdministrador())
            return true;

        return this.Permissoes.Any(p => p.Nome.Equals(nomePermissao, StringComparison.OrdinalIgnoreCase));
    }

    public static List<string> StatusPermitidosAoAlterarStatusDeOutroUsuario()
    {
        return EnumUtil.PegarListaNomesEnum<StatusUsuario>(removerEstes: ["Indefinido", "Excluido"]);
    }

    public static List<string> StatusPermitidosAoAlterarSeuProprioStatus()
    {
        return ["Ativo", "Ausente"];
    }
}
