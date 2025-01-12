namespace Gateway.Domain.Entities.Base;

public interface IIdentifiedEntity
{
    public Guid? Id { get; set; }
}
