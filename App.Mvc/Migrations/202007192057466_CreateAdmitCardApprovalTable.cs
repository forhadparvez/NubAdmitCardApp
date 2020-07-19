namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateAdmitCardApprovalTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdmitCardApprovals",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StudentInfoId = c.Long(nullable: false),
                        IsPaymentComplete = c.Boolean(nullable: false),
                        IsSpecialPermission = c.Boolean(nullable: false),
                        ExceptedDate = c.DateTime(),
                        Comments = c.String(),
                        IsPrevious = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        ApproveBy = c.String(),
                        ApproveDate = c.DateTime(nullable: false),
                        DeleteBy = c.String(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudentInfoes", t => t.StudentInfoId, cascadeDelete: true)
                .Index(t => t.StudentInfoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdmitCardApprovals", "StudentInfoId", "dbo.StudentInfoes");
            DropIndex("dbo.AdmitCardApprovals", new[] { "StudentInfoId" });
            DropTable("dbo.AdmitCardApprovals");
        }
    }
}
