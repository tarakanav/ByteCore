namespace ByteCore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_User_Password : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserModels", "Password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModels", "Password");
        }
    }
}
