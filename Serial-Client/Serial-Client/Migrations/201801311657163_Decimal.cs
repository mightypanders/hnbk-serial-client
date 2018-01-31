namespace Serial_Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Decimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Measurements", "Temperature", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Measurements", "Temperature", c => c.Single(nullable: false));
        }
    }
}
