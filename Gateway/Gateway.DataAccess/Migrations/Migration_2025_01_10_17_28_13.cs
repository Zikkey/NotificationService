using System;
using FluentMigrator;

namespace Gateway.DataAccess.Migrations
{
    [Migration(20250110172813, TransactionBehavior.None, "Added saved_packet table")]
    public class Migration_2025_01_10_17_28_13 : Migration
    {
        public override void Up()
        {
            Execute.Sql("CREATE TYPE saved_packet_state AS ENUM ('new', 'pending', 'sent', 'error');");
            Execute.Sql("CREATE TYPE notification_channel AS ENUM ('email', 'push', 'sms');");

            Create.Table("saved_packet")
                .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("payload").AsCustom("JSONB").NotNullable()
                .WithColumn("created_on").AsDateTimeOffset().NotNullable()
                .WithColumn("state").AsCustom("saved_packet_state").NotNullable().WithDefaultValue("new")
                .WithColumn("error").AsString().Nullable()
                .WithColumn("priority").AsInt32().NotNullable().WithDefaultValue(2)
                .WithColumn("type").AsString().NotNullable()
                .WithColumn("event_id").AsGuid().Nullable()
                .WithColumn("channel").AsCustom("notification_channel").NotNullable();

            Create.Index("IX_saved_packet_state")
                .OnTable("saved_packet")
                .OnColumn("state");
        }

        public override void Down()
        {
            Delete.Index("IX_saved_packet_state").OnTable("saved_packet");
            Delete.Table("saved_packet");
            Execute.Sql("DROP TYPE saved_packet_state");
        }
    }
}
