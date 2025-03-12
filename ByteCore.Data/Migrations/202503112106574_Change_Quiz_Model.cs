namespace ByteCore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Quiz_Model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuizResultModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quiz_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuizModels", t => t.Quiz_Id)
                .ForeignKey("dbo.UserModels", t => t.User_Id)
                .Index(t => t.Quiz_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.QuizResultAnswerModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SelectedOption = c.Int(nullable: false),
                        IsCorrect = c.Boolean(nullable: false),
                        Question_Id = c.Int(),
                        QuizResultModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuestionModels", t => t.Question_Id)
                .ForeignKey("dbo.QuizResultModels", t => t.QuizResultModel_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.QuizResultModel_Id);
            
            AddColumn("dbo.QuizModels", "PassingPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.QuestionModels", "CorrectOption", c => c.Int(nullable: false));
            DropColumn("dbo.QuestionModels", "CorrectOptionIndex");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QuestionModels", "CorrectOptionIndex", c => c.Int(nullable: false));
            DropForeignKey("dbo.QuizResultModels", "User_Id", "dbo.UserModels");
            DropForeignKey("dbo.QuizResultModels", "Quiz_Id", "dbo.QuizModels");
            DropForeignKey("dbo.QuizResultAnswerModels", "QuizResultModel_Id", "dbo.QuizResultModels");
            DropForeignKey("dbo.QuizResultAnswerModels", "Question_Id", "dbo.QuestionModels");
            DropIndex("dbo.QuizResultAnswerModels", new[] { "QuizResultModel_Id" });
            DropIndex("dbo.QuizResultAnswerModels", new[] { "Question_Id" });
            DropIndex("dbo.QuizResultModels", new[] { "User_Id" });
            DropIndex("dbo.QuizResultModels", new[] { "Quiz_Id" });
            DropColumn("dbo.QuestionModels", "CorrectOption");
            DropColumn("dbo.QuizModels", "PassingPercentage");
            DropTable("dbo.QuizResultAnswerModels");
            DropTable("dbo.QuizResultModels");
        }
    }
}
