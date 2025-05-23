namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Data_Annotations_2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AuditLogs", "UserId", "dbo.Users");
            DropIndex("dbo.AuditLogs", new[] { "UserId" });
            AlterColumn("dbo.AuditLogs", "UserId", c => c.Int());
            CreateIndex("dbo.AuditLogs", "UserId");
            AddForeignKey("dbo.AuditLogs", "UserId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuditLogs", "UserId", "dbo.Users");
            DropIndex("dbo.AuditLogs", new[] { "UserId" });
            AlterColumn("dbo.AuditLogs", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.AuditLogs", "UserId");
            AddForeignKey("dbo.AuditLogs", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
