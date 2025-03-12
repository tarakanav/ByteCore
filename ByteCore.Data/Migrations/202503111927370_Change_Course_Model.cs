namespace ByteCore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Course_Model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChapterModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        CourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseModels", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.SectionModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        TextContent = c.String(),
                        VideoUrl = c.String(),
                        Type = c.Int(nullable: false),
                        ChapterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChapterModels", t => t.ChapterId, cascadeDelete: true)
                .Index(t => t.ChapterId);
            
            AlterColumn("dbo.CourseModels", "Duration", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionModels", "ChapterId", "dbo.ChapterModels");
            DropForeignKey("dbo.ChapterModels", "CourseId", "dbo.CourseModels");
            DropIndex("dbo.SectionModels", new[] { "ChapterId" });
            DropIndex("dbo.ChapterModels", new[] { "CourseId" });
            AlterColumn("dbo.CourseModels", "Duration", c => c.String());
            DropTable("dbo.SectionModels");
            DropTable("dbo.ChapterModels");
        }
    }
}
