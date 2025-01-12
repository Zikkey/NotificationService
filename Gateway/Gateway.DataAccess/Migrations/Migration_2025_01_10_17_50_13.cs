using System;
using FluentMigrator;

namespace Gateway.DataAccess.Migrations
{
    [Migration(20250110175013, TransactionBehavior.Default, "Added index on channel type on saved_packet table")]
    public class Migration_2025_01_10_17_50_13 : Migration
    {
        public override void Up()
        {
            Create.Index("IX_saved_packet_channel").OnTable("saved_packet").InSchema("public").OnColumn("channel");
        }

        public override void Down()
        {
            Delete.Index("IX_saved_packet_channel").OnTable("saved_packet").InSchema("public");
        }
    }
}
