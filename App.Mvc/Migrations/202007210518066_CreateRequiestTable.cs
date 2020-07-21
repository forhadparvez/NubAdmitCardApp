namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRequiestTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdmitCardRequests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StudentInfoId = c.Long(nullable: false),
                        RequestedDate = c.DateTime(nullable: false),
                        Comment = c.String(),
                        Status = c.Boolean(nullable: false),
                        IsDone = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudentInfoes", t => t.StudentInfoId, cascadeDelete: true)
                .Index(t => t.StudentInfoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdmitCardRequests", "StudentInfoId", "dbo.StudentInfoes");
            DropIndex("dbo.AdmitCardRequests", new[] { "StudentInfoId" });
            DropTable("dbo.AdmitCardRequests");
        }
    }
}
