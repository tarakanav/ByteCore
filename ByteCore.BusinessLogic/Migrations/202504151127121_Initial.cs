namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Sessions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        ShortDescription = c.String(),
                        Description = c.String(),
                        Instructor = c.String(),
                        Duration = c.Time(nullable: false, precision: 7),
                        StartDate = c.DateTime(nullable: false),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Chapters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        CourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        TextContent = c.String(),
                        VideoUrl = c.String(),
                        Type = c.Int(nullable: false),
                        Chapter_Id = c.Int(),
                        Quiz_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chapters", t => t.Chapter_Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .Index(t => t.Chapter_Id)
                .Index(t => t.Quiz_Id);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        RewardPoints = c.Int(nullable: false),
                        PassingPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(),
                        CorrectOption = c.Int(nullable: false),
                        Quiz_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .Index(t => t.Quiz_Id);
            
            CreateTable(
                "dbo.QuestionOptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OptionText = c.String(),
                        Question_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuizResultAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SelectedOption = c.Int(nullable: false),
                        IsCorrect = c.Boolean(nullable: false),
                        Question_Id = c.Int(),
                        QuizResult_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .ForeignKey("dbo.QuizResults", t => t.QuizResult_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.QuizResult_Id);
            
            CreateTable(
                "dbo.QuizResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quiz_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Quiz_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CookieString = c.String(),
                        ExpireTime = c.DateTime(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserCourses",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Course_Id })
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Course_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserSessions", "User_Id", "dbo.Users");
            DropForeignKey("dbo.QuizResults", "User_Id", "dbo.Users");
            DropForeignKey("dbo.QuizResults", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.QuizResultAnswers", "QuizResult_Id", "dbo.QuizResults");
            DropForeignKey("dbo.QuizResultAnswers", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.UserCourses", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.UserCourses", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Sections", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.Questions", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.QuestionOptions", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.Sections", "Chapter_Id", "dbo.Chapters");
            DropForeignKey("dbo.Chapters", "CourseId", "dbo.Courses");
            DropIndex("dbo.UserCourses", new[] { "Course_Id" });
            DropIndex("dbo.UserCourses", new[] { "User_Id" });
            DropIndex("dbo.UserSessions", new[] { "User_Id" });
            DropIndex("dbo.QuizResults", new[] { "User_Id" });
            DropIndex("dbo.QuizResults", new[] { "Quiz_Id" });
            DropIndex("dbo.QuizResultAnswers", new[] { "QuizResult_Id" });
            DropIndex("dbo.QuizResultAnswers", new[] { "Question_Id" });
            DropIndex("dbo.QuestionOptions", new[] { "Question_Id" });
            DropIndex("dbo.Questions", new[] { "Quiz_Id" });
            DropIndex("dbo.Sections", new[] { "Quiz_Id" });
            DropIndex("dbo.Sections", new[] { "Chapter_Id" });
            DropIndex("dbo.Chapters", new[] { "CourseId" });
            DropTable("dbo.UserCourses");
            DropTable("dbo.UserSessions");
            DropTable("dbo.QuizResults");
            DropTable("dbo.QuizResultAnswers");
            DropTable("dbo.Users");
            DropTable("dbo.QuestionOptions");
            DropTable("dbo.Questions");
            DropTable("dbo.Quizs");
            DropTable("dbo.Sections");
            DropTable("dbo.Chapters");
            DropTable("dbo.Courses");
        }
    }
}
