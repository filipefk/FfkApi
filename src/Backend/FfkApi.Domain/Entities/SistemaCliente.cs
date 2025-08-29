using FfkApi.Domain.Enums;

namespace FfkApi.Domain.Entities;

public class SistemaCliente : EntityBase
{
    public Guid AppId { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public StatusSistemaCliente Status { get; set; } = StatusSistemaCliente.Indefinido;
}
