namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CratePaymentTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProgramId = c.Byte(nullable: false),
                        SemesterId = c.Int(nullable: false),
                        ExamId = c.Byte(nullable: false),
                        StudentId = c.String(),
                        StudentName = c.String(),
                        WaiverPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreditTaken = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreviousDues = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayablePercentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DuesPercentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDues = c.Decimal(nullable: false, precision: 18, scale: 2),
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
            DropForeignKey("dbo.Payments", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.Payments", "ProgramId", "dbo.Programs");
            DropForeignKey("dbo.Payments", "ExamId", "dbo.Exams");
            DropIndex("dbo.Payments", new[] { "ExamId" });
            DropIndex("dbo.Payments", new[] { "SemesterId" });
            DropIndex("dbo.Payments", new[] { "ProgramId" });
            DropTable("dbo.Payments");
        }
    }
}
