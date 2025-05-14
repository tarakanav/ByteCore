namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Added_Data_Annotations_3 : DbMigration
    {
        public override void Up()
        {
            // Drop the old FK constraint
            DropForeignKey("dbo.AuditLogs", "UserId", "dbo.Users");

            // Alter the column to allow NULLs
            AlterColumn("dbo.AuditLogs", "UserId", c => c.Int());

            // Re-create the FK (now optional)
            AddForeignKey("dbo.AuditLogs", "UserId", "dbo.Users", "Id");
        }

        public override void Down()
        {
            // reverse the above
            DropForeignKey("dbo.AuditLogs", "UserId", "dbo.Users");
            AlterColumn("dbo.AuditLogs", "UserId", c => c.Int(nullable: false));
            AddForeignKey("dbo.AuditLogs", "UserId", "dbo.Users", "Id", cascadeDelete: false);
        }
    }
}
