namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStudentTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StudentInfoes", "IdNo", c => c.String(nullable: false));
            AlterColumn("dbo.StudentInfoes", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.StudentInfoes", "ContactNo", c => c.String(nullable: false));
            AlterColumn("dbo.StudentInfoes", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StudentInfoes", "Email", c => c.String());
            AlterColumn("dbo.StudentInfoes", "ContactNo", c => c.String());
            AlterColumn("dbo.StudentInfoes", "Name", c => c.String());
            AlterColumn("dbo.StudentInfoes", "IdNo", c => c.String());
        }
    }
}
