namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Logging_Relation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuditLogs", "UserId", c => c.String());
            AddColumn("dbo.AuditLogs", "ResponseStatusCode", c => c.Int(nullable: false));
            AddColumn("dbo.AuditLogs", "ResponseContentLength", c => c.Long());
            AddColumn("dbo.AuditLogs", "ExecutionTimeMs", c => c.Int());
            AddColumn("dbo.AuditLogs", "User_Id", c => c.Int());
            AddColumn("dbo.LoginLogs", "User_Id", c => c.Int());
            CreateIndex("dbo.AuditLogs", "User_Id");
            CreateIndex("dbo.LoginLogs", "User_Id");
            AddForeignKey("dbo.AuditLogs", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.LoginLogs", "User_Id", "dbo.Users", "Id");
            DropColumn("dbo.AuditLogs", "UserEmail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AuditLogs", "UserEmail", c => c.String());
            DropForeignKey("dbo.LoginLogs", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AuditLogs", "User_Id", "dbo.Users");
            DropIndex("dbo.LoginLogs", new[] { "User_Id" });
            DropIndex("dbo.AuditLogs", new[] { "User_Id" });
            DropColumn("dbo.LoginLogs", "User_Id");
            DropColumn("dbo.AuditLogs", "User_Id");
            DropColumn("dbo.AuditLogs", "ExecutionTimeMs");
            DropColumn("dbo.AuditLogs", "ResponseContentLength");
            DropColumn("dbo.AuditLogs", "ResponseStatusCode");
            DropColumn("dbo.AuditLogs", "UserId");
        }
    }
}
