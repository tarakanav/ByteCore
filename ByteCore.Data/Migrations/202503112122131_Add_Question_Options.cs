namespace ByteCore.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Question_Options : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionOptionModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OptionText = c.String(),
                        QuestionModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuestionModels", t => t.QuestionModel_Id)
                .Index(t => t.QuestionModel_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionOptionModels", "QuestionModel_Id", "dbo.QuestionModels");
            DropIndex("dbo.QuestionOptionModels", new[] { "QuestionModel_Id" });
            DropTable("dbo.QuestionOptionModels");
        }
    }
}
