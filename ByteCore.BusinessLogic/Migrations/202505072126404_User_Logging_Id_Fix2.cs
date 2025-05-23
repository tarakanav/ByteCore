namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Logging_Id_Fix2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserChapters", newName: "ChapterUsers");
            DropForeignKey("dbo.AuditLogs", "User_Id", "dbo.Users");
            DropIndex("dbo.AuditLogs", new[] { "User_Id" });
            RenameColumn(table: "dbo.AuditLogs", name: "User_Id", newName: "UserId");
            DropPrimaryKey("dbo.ChapterUsers");
            AlterColumn("dbo.AuditLogs", "UserId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ChapterUsers", new[] { "Chapter_Id", "User_Id" });
            CreateIndex("dbo.AuditLogs", "UserId");
            AddForeignKey("dbo.AuditLogs", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuditLogs", "UserId", "dbo.Users");
            DropIndex("dbo.AuditLogs", new[] { "UserId" });
            DropPrimaryKey("dbo.ChapterUsers");
            AlterColumn("dbo.AuditLogs", "UserId", c => c.Int());
            AddPrimaryKey("dbo.ChapterUsers", new[] { "User_Id", "Chapter_Id" });
            RenameColumn(table: "dbo.AuditLogs", name: "UserId", newName: "User_Id");
            CreateIndex("dbo.AuditLogs", "User_Id");
            AddForeignKey("dbo.AuditLogs", "User_Id", "dbo.Users", "Id");
            RenameTable(name: "dbo.ChapterUsers", newName: "UserChapters");
        }
    }
}
