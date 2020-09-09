namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CratePaymentTableUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "IsDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "IsDelete");
        }
    }
}
