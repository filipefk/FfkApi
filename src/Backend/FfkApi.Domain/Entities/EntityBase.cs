namespace FfkApi.Domain.Entities;

public class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime DataCriacaoUtc { get; set; } = DateTime.UtcNow;
}
