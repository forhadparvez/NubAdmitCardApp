namespace App.Mvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SemesterTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Semesters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Year = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        AddBy = c.String(),
                        AddDate = c.DateTime(nullable: false),
                        EditBy = c.String(),
                        EditDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Semesters");
        }
    }
}
