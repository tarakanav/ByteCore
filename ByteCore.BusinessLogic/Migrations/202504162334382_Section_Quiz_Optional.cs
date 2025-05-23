namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Section_Quiz_Optional : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sections", "QuizId", "dbo.Quizs");
            DropIndex("dbo.Sections", new[] { "QuizId" });
            AlterColumn("dbo.Sections", "QuizId", c => c.Int());
            CreateIndex("dbo.Sections", "QuizId");
            AddForeignKey("dbo.Sections", "QuizId", "dbo.Quizs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sections", "QuizId", "dbo.Quizs");
            DropIndex("dbo.Sections", new[] { "QuizId" });
            AlterColumn("dbo.Sections", "QuizId", c => c.Int(nullable: false));
            CreateIndex("dbo.Sections", "QuizId");
            AddForeignKey("dbo.Sections", "QuizId", "dbo.Quizs", "Id", cascadeDelete: true);
        }
    }
}
