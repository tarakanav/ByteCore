namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Chapter_Numbers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chapters", "ChapterNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chapters", "ChapterNumber");
        }
    }
}
