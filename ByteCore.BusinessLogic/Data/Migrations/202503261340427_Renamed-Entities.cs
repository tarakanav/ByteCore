namespace ByteCore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedEntities : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CourseModels", newName: "Courses");
            RenameTable(name: "dbo.ChapterModels", newName: "Chapters");
            RenameTable(name: "dbo.SectionModels", newName: "Sections");
            RenameTable(name: "dbo.QuizModels", newName: "Quizs");
            RenameTable(name: "dbo.QuestionModels", newName: "Questions");
            RenameTable(name: "dbo.QuestionOptionModels", newName: "QuestionOptions");
            RenameTable(name: "dbo.UserModels", newName: "Users");
            RenameTable(name: "dbo.QuizResultAnswerModels", newName: "QuizResultAnswers");
            RenameTable(name: "dbo.QuizResultModels", newName: "QuizResults");
            RenameTable(name: "dbo.UserModelCourseModels", newName: "UserCourses");
            RenameColumn(table: "dbo.UserCourses", name: "UserModel_Id", newName: "User_Id");
            RenameColumn(table: "dbo.UserCourses", name: "CourseModel_Id", newName: "Course_Id");
            RenameColumn(table: "dbo.Questions", name: "QuizModel_Id", newName: "Quiz_Id");
            RenameColumn(table: "dbo.QuestionOptions", name: "QuestionModel_Id", newName: "Question_Id");
            RenameColumn(table: "dbo.QuizResultAnswers", name: "QuizResultModel_Id", newName: "QuizResult_Id");
            RenameIndex(table: "dbo.Questions", name: "IX_QuizModel_Id", newName: "IX_Quiz_Id");
            RenameIndex(table: "dbo.QuestionOptions", name: "IX_QuestionModel_Id", newName: "IX_Question_Id");
            RenameIndex(table: "dbo.QuizResultAnswers", name: "IX_QuizResultModel_Id", newName: "IX_QuizResult_Id");
            RenameIndex(table: "dbo.UserCourses", name: "IX_UserModel_Id", newName: "IX_User_Id");
            RenameIndex(table: "dbo.UserCourses", name: "IX_CourseModel_Id", newName: "IX_Course_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.UserCourses", name: "IX_Course_Id", newName: "IX_CourseModel_Id");
            RenameIndex(table: "dbo.UserCourses", name: "IX_User_Id", newName: "IX_UserModel_Id");
            RenameIndex(table: "dbo.QuizResultAnswers", name: "IX_QuizResult_Id", newName: "IX_QuizResultModel_Id");
            RenameIndex(table: "dbo.QuestionOptions", name: "IX_Question_Id", newName: "IX_QuestionModel_Id");
            RenameIndex(table: "dbo.Questions", name: "IX_Quiz_Id", newName: "IX_QuizModel_Id");
            RenameColumn(table: "dbo.QuizResultAnswers", name: "QuizResult_Id", newName: "QuizResultModel_Id");
            RenameColumn(table: "dbo.QuestionOptions", name: "Question_Id", newName: "QuestionModel_Id");
            RenameColumn(table: "dbo.Questions", name: "Quiz_Id", newName: "QuizModel_Id");
            RenameColumn(table: "dbo.UserCourses", name: "Course_Id", newName: "CourseModel_Id");
            RenameColumn(table: "dbo.UserCourses", name: "User_Id", newName: "UserModel_Id");
            RenameTable(name: "dbo.UserCourses", newName: "UserModelCourseModels");
            RenameTable(name: "dbo.QuizResults", newName: "QuizResultModels");
            RenameTable(name: "dbo.QuizResultAnswers", newName: "QuizResultAnswerModels");
            RenameTable(name: "dbo.Users", newName: "UserModels");
            RenameTable(name: "dbo.QuestionOptions", newName: "QuestionOptionModels");
            RenameTable(name: "dbo.Questions", newName: "QuestionModels");
            RenameTable(name: "dbo.Quizs", newName: "QuizModels");
            RenameTable(name: "dbo.Sections", newName: "SectionModels");
            RenameTable(name: "dbo.Chapters", newName: "ChapterModels");
            RenameTable(name: "dbo.Courses", newName: "CourseModels");
        }
    }
}
