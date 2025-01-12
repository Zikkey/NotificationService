using FluentMigrator.Runner.VersionTableInfo;

namespace Gateway.Infrastructure;

public class CustomVersionTable : IVersionTableMetaData
{
    public object ApplicationContext { get; set; }

    public string ColumnName => "version";

    public bool OwnsSchema => true;

    public string SchemaName => "public";

    public string TableName => "migrations_history";

    public string UniqueIndexName => "ux_version";

    public string AppliedOnColumnName => "applied_on";

    public bool CreateWithPrimaryKey => false;

    public string DescriptionColumnName => "description";
}
