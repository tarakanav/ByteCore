namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Audit_Login_Logging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserEmail = c.String(),
                        ActionTime = c.DateTime(nullable: false),
                        ControllerName = c.String(),
                        ActionName = c.String(),
                        HttpMethod = c.String(),
                        UrlAccessed = c.String(),
                        IpAddress = c.String(),
                        UserAgent = c.String(),
                        QueryString = c.String(),
                        FormData = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoginLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        LoginTime = c.DateTime(nullable: false),
                        IpAddress = c.String(),
                        UserAgent = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LoginLogs");
            DropTable("dbo.AuditLogs");
        }
    }
}
