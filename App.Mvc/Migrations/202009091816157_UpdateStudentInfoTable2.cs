namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStudentInfoTable2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudentInfoes", "ExamId", "dbo.Exams");
            DropForeignKey("dbo.StudentInfoes", "SemesterId", "dbo.Semesters");
            DropIndex("dbo.StudentInfoes", new[] { "SemesterId" });
            DropIndex("dbo.StudentInfoes", new[] { "ExamId" });
            DropColumn("dbo.StudentInfoes", "SemesterId");
            DropColumn("dbo.StudentInfoes", "ExamId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StudentInfoes", "ExamId", c => c.Byte(nullable: false));
            AddColumn("dbo.StudentInfoes", "SemesterId", c => c.Int(nullable: false));
            CreateIndex("dbo.StudentInfoes", "ExamId");
            CreateIndex("dbo.StudentInfoes", "SemesterId");
            AddForeignKey("dbo.StudentInfoes", "SemesterId", "dbo.Semesters", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StudentInfoes", "ExamId", "dbo.Exams", "Id", cascadeDelete: true);
        }
    }
}
