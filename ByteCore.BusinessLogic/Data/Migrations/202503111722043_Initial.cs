namespace ByteCore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        ShortDescription = c.String(),
                        Description = c.String(),
                        Instructor = c.String(),
                        Duration = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        ImageUrl = c.String(),
                        UserModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserModels", t => t.UserModel_Id)
                .Index(t => t.UserModel_Id);
            
            CreateTable(
                "dbo.QuizModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        RewardPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(),
                        CorrectOptionIndex = c.Int(nullable: false),
                        QuizModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuizModels", t => t.QuizModel_Id)
                .Index(t => t.QuizModel_Id);
            
            CreateTable(
                "dbo.UserModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseModels", "UserModel_Id", "dbo.UserModels");
            DropForeignKey("dbo.QuestionModels", "QuizModel_Id", "dbo.QuizModels");
            DropIndex("dbo.QuestionModels", new[] { "QuizModel_Id" });
            DropIndex("dbo.CourseModels", new[] { "UserModel_Id" });
            DropTable("dbo.UserModels");
            DropTable("dbo.QuestionModels");
            DropTable("dbo.QuizModels");
            DropTable("dbo.CourseModels");
        }
    }
}
