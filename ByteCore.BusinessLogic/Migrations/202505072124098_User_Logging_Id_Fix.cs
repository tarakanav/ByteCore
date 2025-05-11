namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Logging_Id_Fix : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AuditLogs", "UserId");
            DropColumn("dbo.LoginLogs", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LoginLogs", "UserId", c => c.String());
            AddColumn("dbo.AuditLogs", "UserId", c => c.String());
        }
    }
}
