using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UI.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tbl_BuyType",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_BuyType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_PaymentMethod",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_PaymentMethod", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_eVoucher",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    PaymentMethodID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BuyTypeID = table.Column<int>(type: "int", nullable: false),
                    InActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_eVoucher", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tbl_eVoucher_Tbl_BuyType_BuyTypeID",
                        column: x => x.BuyTypeID,
                        principalTable: "Tbl_BuyType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tbl_eVoucher_Tbl_PaymentMethod_PaymentMethodID",
                        column: x => x.PaymentMethodID,
                        principalTable: "Tbl_PaymentMethod",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_eVoucher_BuyTypeID",
                table: "Tbl_eVoucher",
                column: "BuyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_eVoucher_PaymentMethodID",
                table: "Tbl_eVoucher",
                column: "PaymentMethodID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tbl_eVoucher");

            migrationBuilder.DropTable(
                name: "Tbl_BuyType");

            migrationBuilder.DropTable(
                name: "Tbl_PaymentMethod");
        }
    }
}
