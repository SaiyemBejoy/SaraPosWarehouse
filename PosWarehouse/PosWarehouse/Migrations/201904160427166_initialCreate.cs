namespace PosWarehouse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "POSWAREHOUSE.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "POSWAREHOUSE.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("POSWAREHOUSE.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("POSWAREHOUSE.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "POSWAREHOUSE.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Decimal(nullable: false, precision: 1, scale: 0),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Decimal(nullable: false, precision: 1, scale: 0),
                        TwoFactorEnabled = c.Decimal(nullable: false, precision: 1, scale: 0),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Decimal(nullable: false, precision: 1, scale: 0),
                        AccessFailedCount = c.Decimal(nullable: false, precision: 10, scale: 0),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "POSWAREHOUSE.AspNetUserClaims",
                c => new
                    {
                        Id = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("POSWAREHOUSE.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "POSWAREHOUSE.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("POSWAREHOUSE.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("POSWAREHOUSE.AspNetUserRoles", "UserId", "POSWAREHOUSE.AspNetUsers");
            DropForeignKey("POSWAREHOUSE.AspNetUserLogins", "UserId", "POSWAREHOUSE.AspNetUsers");
            DropForeignKey("POSWAREHOUSE.AspNetUserClaims", "UserId", "POSWAREHOUSE.AspNetUsers");
            DropForeignKey("POSWAREHOUSE.AspNetUserRoles", "RoleId", "POSWAREHOUSE.AspNetRoles");
            DropIndex("POSWAREHOUSE.AspNetUserLogins", new[] { "UserId" });
            DropIndex("POSWAREHOUSE.AspNetUserClaims", new[] { "UserId" });
            DropIndex("POSWAREHOUSE.AspNetUsers", "UserNameIndex");
            DropIndex("POSWAREHOUSE.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("POSWAREHOUSE.AspNetUserRoles", new[] { "UserId" });
            DropIndex("POSWAREHOUSE.AspNetRoles", "RoleNameIndex");
            DropTable("POSWAREHOUSE.AspNetUserLogins");
            DropTable("POSWAREHOUSE.AspNetUserClaims");
            DropTable("POSWAREHOUSE.AspNetUsers");
            DropTable("POSWAREHOUSE.AspNetUserRoles");
            DropTable("POSWAREHOUSE.AspNetRoles");
        }
    }
}
