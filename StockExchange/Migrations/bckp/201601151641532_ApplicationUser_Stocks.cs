namespace StockExchange.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationUser_Stocks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stocks", "Owner_Id", "dbo.AspNetUsers");
            AddColumn("dbo.Stocks", "ApplicationUser_Id", c => c.String(maxLength: 128));
           // AddColumn("dbo.Stocks", "ApplicationUser_Id1", c => c.String(maxLength: 128));
            CreateIndex("dbo.Stocks", "ApplicationUser_Id");
          //  CreateIndex("dbo.Stocks", "ApplicationUser_Id1");
            AddForeignKey("dbo.Stocks", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            //AddForeignKey("dbo.Stocks", "ApplicationUser_Id1", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Stocks", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.Stocks", "ApplicationUser_Id", "dbo.AspNetUsers");
           // DropIndex("dbo.Stocks", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Stocks", new[] { "ApplicationUser_Id" });
           // DropColumn("dbo.Stocks", "ApplicationUser_Id1");
            DropColumn("dbo.Stocks", "ApplicationUser_Id");
            AddForeignKey("dbo.Stocks", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
