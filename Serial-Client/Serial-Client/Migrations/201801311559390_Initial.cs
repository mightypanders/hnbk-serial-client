namespace Serial_Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Room = c.String(),
                        PcNumber = c.String(),
                        LocationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId, cascadeDelete: true)
                .Index(t => t.LocationId);
            
            CreateTable(
                "dbo.Measurements",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Temperature = c.Single(nullable: false),
                        PositionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Positions", t => t.PositionId, cascadeDelete: true)
                .Index(t => t.PositionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Measurements", "PositionId", "dbo.Positions");
            DropForeignKey("dbo.Positions", "LocationId", "dbo.Locations");
            DropIndex("dbo.Measurements", new[] { "PositionId" });
            DropIndex("dbo.Positions", new[] { "LocationId" });
            DropTable("dbo.Measurements");
            DropTable("dbo.Positions");
            DropTable("dbo.Locations");
        }
    }
}
