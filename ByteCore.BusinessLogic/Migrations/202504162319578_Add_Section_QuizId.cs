namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Section_QuizId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sections", "Quiz_Id", "dbo.Quizs");
            DropIndex("dbo.Sections", new[] { "Quiz_Id" });
            RenameColumn(table: "dbo.Sections", name: "Quiz_Id", newName: "QuizId");
            AlterColumn("dbo.Sections", "QuizId", c => c.Int(nullable: false));
            CreateIndex("dbo.Sections", "QuizId");
            AddForeignKey("dbo.Sections", "QuizId", "dbo.Quizs", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sections", "QuizId", "dbo.Quizs");
            DropIndex("dbo.Sections", new[] { "QuizId" });
            AlterColumn("dbo.Sections", "QuizId", c => c.Int());
            RenameColumn(table: "dbo.Sections", name: "QuizId", newName: "Quiz_Id");
            CreateIndex("dbo.Sections", "Quiz_Id");
            AddForeignKey("dbo.Sections", "Quiz_Id", "dbo.Quizs", "Id");
        }
    }
}
