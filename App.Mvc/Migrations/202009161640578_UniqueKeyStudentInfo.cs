namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueKeyStudentInfo : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StudentInfoes", "IdNo", c => c.String(nullable: false, maxLength: 15));
            CreateIndex("dbo.StudentInfoes", "IdNo", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.StudentInfoes", new[] { "IdNo" });
            AlterColumn("dbo.StudentInfoes", "IdNo", c => c.String(nullable: false));
        }
    }
}
