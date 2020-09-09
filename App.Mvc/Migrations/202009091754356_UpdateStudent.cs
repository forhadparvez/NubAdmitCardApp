namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStudent : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StudentInfoes", "PaymentFilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StudentInfoes", "PaymentFilePath", c => c.String());
        }
    }
}
