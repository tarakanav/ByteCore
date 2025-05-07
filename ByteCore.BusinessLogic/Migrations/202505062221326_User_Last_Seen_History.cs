namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Last_Seen_History : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserLastSeens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeenOn = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserLastSeens", "UserId", "dbo.Users");
            DropIndex("dbo.UserLastSeens", new[] { "UserId" });
            DropTable("dbo.UserLastSeens");
        }
    }
}
