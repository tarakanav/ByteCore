namespace ByteCore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Chapter_Model : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SectionModels", "ChapterId", "dbo.ChapterModels");
            DropIndex("dbo.SectionModels", new[] { "ChapterId" });
            RenameColumn(table: "dbo.SectionModels", name: "ChapterId", newName: "Chapter_Id");
            AddColumn("dbo.SectionModels", "Quiz_Id", c => c.Int());
            AlterColumn("dbo.SectionModels", "Chapter_Id", c => c.Int());
            CreateIndex("dbo.SectionModels", "Chapter_Id");
            CreateIndex("dbo.SectionModels", "Quiz_Id");
            AddForeignKey("dbo.SectionModels", "Quiz_Id", "dbo.QuizModels", "Id");
            AddForeignKey("dbo.SectionModels", "Chapter_Id", "dbo.ChapterModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionModels", "Chapter_Id", "dbo.ChapterModels");
            DropForeignKey("dbo.SectionModels", "Quiz_Id", "dbo.QuizModels");
            DropIndex("dbo.SectionModels", new[] { "Quiz_Id" });
            DropIndex("dbo.SectionModels", new[] { "Chapter_Id" });
            AlterColumn("dbo.SectionModels", "Chapter_Id", c => c.Int(nullable: false));
            DropColumn("dbo.SectionModels", "Quiz_Id");
            RenameColumn(table: "dbo.SectionModels", name: "Chapter_Id", newName: "ChapterId");
            CreateIndex("dbo.SectionModels", "ChapterId");
            AddForeignKey("dbo.SectionModels", "ChapterId", "dbo.ChapterModels", "Id", cascadeDelete: true);
        }
    }
}
