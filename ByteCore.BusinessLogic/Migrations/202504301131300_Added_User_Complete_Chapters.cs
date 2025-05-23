namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_User_Complete_Chapters : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Chapter_Id", "dbo.Chapters");
            DropIndex("dbo.Users", new[] { "Chapter_Id" });
            CreateTable(
                "dbo.UserChapters",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Chapter_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Chapter_Id })
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Chapters", t => t.Chapter_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Chapter_Id);
            
            DropColumn("dbo.Users", "Chapter_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Chapter_Id", c => c.Int());
            DropForeignKey("dbo.UserChapters", "Chapter_Id", "dbo.Chapters");
            DropForeignKey("dbo.UserChapters", "User_Id", "dbo.Users");
            DropIndex("dbo.UserChapters", new[] { "Chapter_Id" });
            DropIndex("dbo.UserChapters", new[] { "User_Id" });
            DropTable("dbo.UserChapters");
            CreateIndex("dbo.Users", "Chapter_Id");
            AddForeignKey("dbo.Users", "Chapter_Id", "dbo.Chapters", "Id");
        }
    }
}
