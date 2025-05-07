namespace ByteCore.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Browser_LastSeen : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserCourses", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserCourses", "Course_Id", "dbo.Courses");
            DropIndex("dbo.UserCourses", new[] { "User_Id" });
            DropIndex("dbo.UserCourses", new[] { "Course_Id" });
            DropTable("dbo.UserCourses");
            CreateTable(
                "dbo.UserCourses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        EnrolledDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.CourseId);
            
            AddColumn("dbo.Users", "LatestBrowserUsed", c => c.String());
            AddColumn("dbo.Users", "RegistrationTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "LastSeen", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserCourses",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Course_Id });
            
            DropForeignKey("dbo.UserCourses", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserCourses", "CourseId", "dbo.Courses");
            DropIndex("dbo.UserCourses", new[] { "CourseId" });
            DropIndex("dbo.UserCourses", new[] { "UserId" });
            DropColumn("dbo.Users", "LastSeen");
            DropColumn("dbo.Users", "RegistrationTime");
            DropColumn("dbo.Users", "LatestBrowserUsed");
            DropTable("dbo.UserCourses");
            CreateIndex("dbo.UserCourses", "Course_Id");
            CreateIndex("dbo.UserCourses", "User_Id");
            AddForeignKey("dbo.UserCourses", "Course_Id", "dbo.Courses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserCourses", "User_Id", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
