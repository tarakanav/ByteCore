namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Chapter_Completion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Chapter_Id", c => c.Int());
            CreateIndex("dbo.Users", "Chapter_Id");
            AddForeignKey("dbo.Users", "Chapter_Id", "dbo.Chapters", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Chapter_Id", "dbo.Chapters");
            DropIndex("dbo.Users", new[] { "Chapter_Id" });
            DropColumn("dbo.Users", "Chapter_Id");
        }
    }
}
