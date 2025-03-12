namespace ByteCore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Course_EnrolledUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CourseModels", "UserModel_Id", "dbo.UserModels");
            DropIndex("dbo.CourseModels", new[] { "UserModel_Id" });
            CreateTable(
                "dbo.UserModelCourseModels",
                c => new
                    {
                        UserModel_Id = c.Int(nullable: false),
                        CourseModel_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserModel_Id, t.CourseModel_Id })
                .ForeignKey("dbo.UserModels", t => t.UserModel_Id, cascadeDelete: true)
                .ForeignKey("dbo.CourseModels", t => t.CourseModel_Id, cascadeDelete: true)
                .Index(t => t.UserModel_Id)
                .Index(t => t.CourseModel_Id);
            
            DropColumn("dbo.CourseModels", "UserModel_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CourseModels", "UserModel_Id", c => c.Int());
            DropForeignKey("dbo.UserModelCourseModels", "CourseModel_Id", "dbo.CourseModels");
            DropForeignKey("dbo.UserModelCourseModels", "UserModel_Id", "dbo.UserModels");
            DropIndex("dbo.UserModelCourseModels", new[] { "CourseModel_Id" });
            DropIndex("dbo.UserModelCourseModels", new[] { "UserModel_Id" });
            DropTable("dbo.UserModelCourseModels");
            CreateIndex("dbo.CourseModels", "UserModel_Id");
            AddForeignKey("dbo.CourseModels", "UserModel_Id", "dbo.UserModels", "Id");
        }
    }
}
