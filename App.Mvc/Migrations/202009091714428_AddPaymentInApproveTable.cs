namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPaymentInApproveTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdmitCardApprovals", "PaymentId", c => c.Long());
            CreateIndex("dbo.AdmitCardApprovals", "PaymentId");
            AddForeignKey("dbo.AdmitCardApprovals", "PaymentId", "dbo.Payments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdmitCardApprovals", "PaymentId", "dbo.Payments");
            DropIndex("dbo.AdmitCardApprovals", new[] { "PaymentId" });
            DropColumn("dbo.AdmitCardApprovals", "PaymentId");
        }
    }
}
