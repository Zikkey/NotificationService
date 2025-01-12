using Gateway.Domain.Entities.Base;

namespace Gateway.Domain.Extensions;

public static class EntityExtensions
{
    public static TEntity HandleIdentifiedEntity<TEntity>(this TEntity entity) where TEntity : IIdentifiedEntity
    {
        entity.Id ??= Guid.NewGuid();
        return entity;
    }

    public static TEntity HandleCreatedAuditedEntity<TEntity>(this TEntity entity) where TEntity : ICreateAuditedEntity
    {
        entity.CreatedOn ??= DateTimeOffset.UtcNow;
        return entity;
    }
}
