using System;
using FluentMigrator;

namespace Gateway.DataAccess.Migrations
{
    [Migration(20250110173013, TransactionBehavior.Default, "Added index on priority, created_on columns on saved_packet table")]
    public class Migration_2025_01_10_17_30_13 : Migration
    {
        public override void Up()
        {
            Create.Index("IX_saved_packet_created_on").OnTable("saved_packet").InSchema("public")
                .OnColumn("created_on").Ascending()
                .OnColumn("priority").Ascending();
        }

        public override void Down()
        {
            Delete.Index("IX_saved_packet_created_on").OnTable("saved_packet").InSchema("public");
        }
    }
}
