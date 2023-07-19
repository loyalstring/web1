using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JewelleryWebApplication.Migrations
{
    public partial class new2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PinCode",
                table: "tblCustomerDetails",
                newName: "perAddTown");

            migrationBuilder.RenameColumn(
                name: "PerAdd",
                table: "tblCustomerDetails",
                newName: "perAddStreet");

            migrationBuilder.RenameColumn(
                name: "CurrAdd",
                table: "tblCustomerDetails",
                newName: "perAddState");

            migrationBuilder.AddColumn<string>(
                name: "currAddPinCode",
                table: "tblCustomerDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currAddState",
                table: "tblCustomerDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currAddStreet",
                table: "tblCustomerDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currAddTown",
                table: "tblCustomerDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "perAddPinCode",
                table: "tblCustomerDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrossWt",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MRP",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetWt",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rate",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoneWt",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tblProductsDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    purity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    barcodeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    itemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    box = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    grossWeight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    netWeight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    stoneweight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    makinggm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    makingper = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fixedamount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fixedwastage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    stoneamount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mrp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hudicode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    partycode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusType = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblProductsDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblProductsDetails");

            migrationBuilder.DropColumn(
                name: "currAddPinCode",
                table: "tblCustomerDetails");

            migrationBuilder.DropColumn(
                name: "currAddState",
                table: "tblCustomerDetails");

            migrationBuilder.DropColumn(
                name: "currAddStreet",
                table: "tblCustomerDetails");

            migrationBuilder.DropColumn(
                name: "currAddTown",
                table: "tblCustomerDetails");

            migrationBuilder.DropColumn(
                name: "perAddPinCode",
                table: "tblCustomerDetails");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "GrossWt",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "MRP",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "NetWt",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "StoneWt",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "perAddTown",
                table: "tblCustomerDetails",
                newName: "PinCode");

            migrationBuilder.RenameColumn(
                name: "perAddStreet",
                table: "tblCustomerDetails",
                newName: "PerAdd");

            migrationBuilder.RenameColumn(
                name: "perAddState",
                table: "tblCustomerDetails",
                newName: "CurrAdd");
        }
    }
}
