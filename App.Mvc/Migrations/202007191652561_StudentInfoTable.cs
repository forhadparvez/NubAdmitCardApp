namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentInfoTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProgramId = c.Byte(nullable: false),
                        SemesterId = c.Int(nullable: false),
                        ExamId = c.Byte(nullable: false),
                        IdNo = c.String(),
                        Name = c.String(),
                        ContactNo = c.String(),
                        Email = c.String(),
                        ImageFilePath = c.String(),
                        PaymentFilePath = c.String(),
                        IsDelete = c.Boolean(nullable: false),
                        EditBy = c.String(),
                        EditDate = c.DateTime(),
                        DeleteBy = c.String(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Exams", t => t.ExamId, cascadeDelete: true)
                .ForeignKey("dbo.Programs", t => t.ProgramId, cascadeDelete: true)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.ProgramId)
                .Index(t => t.SemesterId)
                .Index(t => t.ExamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentInfoes", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.StudentInfoes", "ProgramId", "dbo.Programs");
            DropForeignKey("dbo.StudentInfoes", "ExamId", "dbo.Exams");
            DropIndex("dbo.StudentInfoes", new[] { "ExamId" });
            DropIndex("dbo.StudentInfoes", new[] { "SemesterId" });
            DropIndex("dbo.StudentInfoes", new[] { "ProgramId" });
            DropTable("dbo.StudentInfoes");
        }
    }
}
