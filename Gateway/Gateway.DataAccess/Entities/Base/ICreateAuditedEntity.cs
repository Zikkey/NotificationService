namespace Gateway.Domain.Entities.Base;

public interface ICreateAuditedEntity
{
    public DateTimeOffset? CreatedOn { get; set; }
}
